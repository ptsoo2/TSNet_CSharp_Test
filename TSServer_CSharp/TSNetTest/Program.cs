using System.Diagnostics;
using System.Net.Sockets;
using TSNet;
using TSUtil;
using Serilog;
using Serilog.Events;

namespace TSNetTest
{
	// 스레드 풀 위에서 실행되는 동안 최대 동시성 수준을 보장하는 작업 스케줄러를 제공합니다.
	public class LimitedConcurrencyLevelTaskScheduler : TaskScheduler
	{
		// 현재 스레드에서 작업 항목을 처리하고 있는지 여부를 나타냅니다.
		[ThreadStatic]
		private static bool currentThreadIsProcessingItems_;

		// 실행할 태스크 목록
		private readonly LinkedList<Task> lskTask_ = new LinkedList<Task>(); // protected by lock(_tasks)

		// 이 스케줄러가 허용하는 최대 동시성 수준입니다.
		private readonly int maxDegreeOfParallelism_;

		// 스케줄러가 현재 작업 항목을 처리하고 있는지 여부를 나타냅니다.
		private int delegatesQueuedOrRunning_ = 0;

		// 지정한 병렬도로 새 인스턴스를 만듭니다.
		public LimitedConcurrencyLevelTaskScheduler(int maxDegreeOfParallelism)
		{
			if (maxDegreeOfParallelism < 1)
				throw new ArgumentOutOfRangeException("maxDegreeOfParallelism");

			maxDegreeOfParallelism_ = maxDegreeOfParallelism;
		}

		// 스케줄러에 작업을 대기열에 넣습니다.
		protected sealed override void QueueTask(Task task)
		{
			// 처리할 작업 목록에 작업을 추가합니다.
			// 작업을 처리하기 위해 현재 대기 중이거나 실행 중인 delegate 가 충분하지 않으면 다른 작업을 예약합니다.
			lock (lskTask_)
			{
				lskTask_.AddLast(task);
				if (delegatesQueuedOrRunning_ < maxDegreeOfParallelism_)
				{
					++delegatesQueuedOrRunning_;
					_NotifyThreadPoolOfPendingWork();
				}
			}
		}

		// 이 스케줄러에 대해 실행할 작업이 있음을 ThreadPool에 알립니다.
		private void _NotifyThreadPoolOfPendingWork()
		{
			ThreadPool.UnsafeQueueUserWorkItem(_ =>
			{
				// 현재 스레드에서 작업 항목을 처리하고 있습니다.
				// 이것은 이 스레드에 작업의 inlining 를 활성화하는 데 필요합니다.
				currentThreadIsProcessingItems_ = true;
				try
				{
					// 대기열에서 사용 가능한 모든 항목을 처리합니다.
					while (true)
					{
						Task? item = null;
						lock (lskTask_)
						{
							//if (lskTask_ is null)
							//	break;

							// 더 이상 처리할 항목이 없을 때는 처리가 완료되었음을 참고하고 나가십시오.
							if (lskTask_.Count == 0)
							{
								--delegatesQueuedOrRunning_;
								break;
							}

							// 대기열에서 다음 항목 가져오기
							item = lskTask_.First?.Value;
							lskTask_.RemoveFirst();
						}

						// 대기열에서 꺼낸 작업 실행
						if (item is not null)
							base.TryExecuteTask(item);
					}
				}
				// 현재 스레드에서 항목 처리를 완료했습니다.
				finally { currentThreadIsProcessingItems_ = false; }
			}, null);
		}

		// 현재 스레드에서 지정한 작업을 실행하려고 합니다.
		protected sealed override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
		{
			// 이 스레드가 아직 작업을 처리하지 않는 경우 인라인을 지원하지 않습니다.
			if (!currentThreadIsProcessingItems_) return false;

			// If the task was previously queued, remove it from the queue
			if (taskWasPreviouslyQueued)
				// Try to run the task.
				if (TryDequeue(task))
					return base.TryExecuteTask(task);
				else
					return false;
			else
				return base.TryExecuteTask(task);
		}

		// Attempt to remove a previously scheduled task from the scheduler.
		protected sealed override bool TryDequeue(Task task)
		{
			lock (lskTask_) return lskTask_.Remove(task);
		}

		// Gets the maximum concurrency level supported by this scheduler.
		public sealed override int MaximumConcurrencyLevel { get { return maxDegreeOfParallelism_; } }

		// Gets an enumerable of the tasks currently scheduled on this scheduler.
		protected sealed override IEnumerable<Task> GetScheduledTasks()
		{
			bool lockTaken = false;
			try
			{
				Monitor.TryEnter(lskTask_, ref lockTaken);
				if (lockTaken) return lskTask_;
				else throw new NotSupportedException();
			}
			finally
			{
				if (lockTaken) Monitor.Exit(lskTask_);
			}
		}
	}

	internal class Program
	{
		static Process nowProcess_ = Process.GetCurrentProcess();
		static Latch waitControl_ = new();
		static CAcceptorBase? acceptor_ = null;

		static readonly CPerformanceMeasurer_MT acceptPerformanceMeasurer = new();

		public static void launchMonitor(int intervalMilliSec = 1000)
		{
			// monitor
			Task.Factory.StartNew(
				() =>
				{
					ThreadUtil.setThreadName("monitor");

					PerformanceProperties properties;
					double peakCPUUsage = 0;

					while (waitControl_.isEnd() == false)
					{
						if (acceptPerformanceMeasurer.update(intervalMilliSec, out properties) == true)
						{
							nowProcess_.Refresh();

							// Accept
							{
								LOG.INFO($"[Acceptor] Accept socket(now: {properties.nowCount_.ToString()}, total: {properties.totalCount_.ToString()})");
							}

							// CPU
							{
								double nowCpuUsage = CSystem.GetCpuUsageForProcess(nowProcess_, intervalMilliSec);
								peakCPUUsage = double.Max(nowCpuUsage, peakCPUUsage);

								string descCpuUsage = $"""
													CPU: {nowCpuUsage.ToString("0.00")} 
													| PeakCPU: {peakCPUUsage.ToString("0.00")}
													""";
								LOG.INFO($"[CPU] {descCpuUsage.Replace(Environment.NewLine, "")}");
							}

							// Memory
							{
								string descMemoryUsage = $"""
														PeakVirtual: {nowProcess_.PeakVirtualMemorySize64.BYTE_TO_GIGABYTE().ToString()} 
														| PeakPaged: {nowProcess_.PeakPagedMemorySize64.BYTE_TO_MEGABYTE().ToString()} 
														| PeakWorkingSet: {nowProcess_.PeakWorkingSet64.BYTE_TO_MEGABYTE().ToString()} 
														| GC: {GC.GetTotalMemory(true).BYTE_TO_GIGABYTE().ToString()}
														""";
								LOG.INFO($"[Memory] {descMemoryUsage.Replace(Environment.NewLine, "")}");
							}

							// WorkQueueCount
							{
								LOG.INFO($"[Thread] Thread: {ThreadPool.ThreadCount.ToString()}, Complete: {ThreadPool.CompletedWorkItemCount.ToString()}, Pending: {ThreadPool.PendingWorkItemCount.ToString()}");
							}
						}

						Thread.Sleep(1);
					}
				}, TaskCreationOptions.LongRunning
			);
		}

		static void startup()
		{
			//string outputTemplate = "{{\"dt\":\"{Timestamp:yyyy-MM-ddTHH:mm:ss.fff}\",\"lv\":\"{Level:u4}\",\"fn\":\"{SourceLoc}\",\"thr\":\"{ThreadId}({ThreadName})\",\"ct\":\"{Message:lj}\"}}{NewLine}{Exception}";
			string outputTemplate = "{{\"dt\":\"{Timestamp:yyyy-MM-ddTHH:mm:ss.fff}\",\"lv\":\"{Level:u4}\",\"fn\":\"{SourceLoc}\",\"ct\":\"{Message:lj}\"}}{NewLine}{Exception}";

			LoggerConfiguration loggerConfig = new LoggerConfiguration();
			Log.Logger = loggerConfig
				.MinimumLevel.Is(LogEventLevel.Verbose)
				.WriteTo.Console(LogEventLevel.Verbose, outputTemplate)
				.Enrich.FromLogContext()
				//.Enrich.WithThreadId()
				//.Enrich.WithThreadName()
				.CreateLogger();

			ThreadUtil.setThreadName("main");
			LOG.DEBUG($"Start of main(lang version: {Environment.Version.ToString()})");

			// launch stop thread
			Task.Factory.StartNew(
				() =>
				{
					while (Console.KeyAvailable == false)
						Thread.Sleep(500);

					waitControl_.countdown();
				}, TaskCreationOptions.LongRunning
			);

			launchMonitor(5.SEC_TO_MILLISEC());
		}

		static void cleanup()
		{
			acceptor_?.stop();
		}

		static void Main(string[] args)
		{
#pragma warning disable CS0162
			startup();

			acceptor_ = CAcceptorFactory.create<CAcceptor_Async_TAP>(new CAcceptorConfig { port = 30002, },
				(Socket socket) =>
				{
					acceptPerformanceMeasurer.incrementCount();

					socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, new LingerOption(true, 0));
					LOG.DEBUG(socket.RemoteEndPoint?.ToString() ?? "unknown");
				}
			);

			acceptor_?.start();
			waitControl_.wait();

			cleanup();
#pragma warning restore CS0162
		}
	}
}
