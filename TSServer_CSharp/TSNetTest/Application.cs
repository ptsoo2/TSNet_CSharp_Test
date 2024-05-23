using System.Diagnostics;
using System.Net.Sockets;
using TSNet;
using TSUtil;

namespace TSNetTest
{
	internal partial class Application
	{
		public static void launchSubThread(int intervalMilliSec = 1000)
		{
			// launch stop thread
			Task.Factory.StartNew(
				() =>
				{
					while (Console.KeyAvailable == false)
						Thread.Sleep(500);

					CNetworkService.instance.stop();
				}, TaskCreationOptions.LongRunning
			).DetectThrowOnDispose();

			// monitor
			if (intervalMilliSec > 0)
			{
				Task.Factory.StartNew(
					() =>
					{
						ThreadUtil.setThreadName("monitor");

						PerformanceEvent perfEvent;
						double peakCPUUsage = 0;

						while (CNetworkService.instance.running == true)
						{
							if (acceptPerformanceMeasurer.capture(intervalMilliSec, out perfEvent) == true)
							{
								nowProcess_.Refresh();

								// Accept
								{
									LOG.INFO($"Perf Acceptor(id: {perfEvent.eventId_}, now: {perfEvent.nowCount_.ToString()}, avg: {perfEvent.avgCount_.ToString()}, total: {perfEvent.totalCount_.ToString()})");
								}

								// CPU
								{
									double nowCpuUsage = CSystem.getCpuUsageForProcess(nowProcess_, intervalMilliSec);
									peakCPUUsage = double.Max(nowCpuUsage, peakCPUUsage);

									// LOG.INFO($"Perf CPU(now: {nowCpuUsage.ToString("0.00")}, peak: {peakCPUUsage.ToString("0.00")})");
								}

								// Memory
								{
									string descMemoryUsage = $"""
														PeakVirtual: {nowProcess_.PeakVirtualMemorySize64.BYTE_TO_GIGABYTE().ToString()}, 
														PeakPaged: {nowProcess_.PeakPagedMemorySize64.BYTE_TO_MEGABYTE().ToString()}, 
														PeakWorkingSet: {nowProcess_.PeakWorkingSet64.BYTE_TO_MEGABYTE().ToString()}, 
														GC: {GC.GetTotalMemory(true).BYTE_TO_GIGABYTE().ToString()}
														""";
									// LOG.INFO($"Perf Memory({descMemoryUsage.Replace(Environment.NewLine, "")})");
								}

								// WorkQueueCount
								{
									// LOG.INFO($"Perf Thread(threadCount: {ThreadPool.ThreadCount.ToString()}, completeWork: {ThreadPool.CompletedWorkItemCount.ToString()}, pendingWork: {ThreadPool.PendingWorkItemCount.ToString()})");
								}
							}

							Thread.Sleep(1);
						}
					}, TaskCreationOptions.LongRunning
				).DetectThrowOnDispose();
			}
		}
	}

	internal partial class Application
	{
		static Process nowProcess_ = Process.GetCurrentProcess();
		static readonly CPerformanceMeasurer_MT acceptPerformanceMeasurer = new();
	}

	internal partial class Application
	{
		static void startup()
		{
			ThreadUtil.setThreadName("main");

			launchSubThread(-1);

			var clientSocketConfig = new CSocketOptionConfig { lingerOption_ = new LingerOption(true, 0), };

			CNetworkService.instance
				.init()
				.addAcceptor(
					new CAcceptorConfig
					{
						socketOption_ = new CSocketOptionConfig { reuseAddress_ = true },
						port_ = 30002,
					}, (Socket socket) =>
					{
						acceptPerformanceMeasurer.incrementCount();

						// 소켓 멀쩡한지 확인하고,
						if (socket == null)
						{
							LOG.ERROR("Socket is null");
							return;
						}

						if (socket.Connected == false)
						{
							LOG.ERROR($"Not connected socket({socket.Handle.ToString()})");
							socket.Dispose();
							return;
						}

						// 소켓 옵션 조정하고,
						socket.configureOption(clientSocketConfig);

						// ptsoo todo - 암호화 방식 결정

						// 세션 할당해서 붙여주고, 시작
						CTCPSession session = new CTCPSession();
						session.start(socket, 1 << 14); // 16384

						LOG.VERBOSE(socket.RemoteEndPoint?.ToString() ?? "unknown");
					}
				).launch();
		}

		static void cleanup()
		{
			CNetworkService.instance.stop();
		}

		static unsafe void Main(string[] args)
		{
#pragma warning disable CS0162 // 테스트 용도
			startup();
			cleanup();
#pragma warning restore CS0162
		}
	}
}
