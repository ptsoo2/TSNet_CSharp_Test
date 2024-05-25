using System.Diagnostics;
using System.Net.Sockets;
using TSNet;
using TSServerCommon;
using TSUtil;

namespace TSNetTest
{
	internal partial class CApplication
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
						double peakCPUUsage = 0;
						Stopwatch watch = new();
						watch.Restart();

						while (CNetworkService.instance.running == true)
						{
							if (watch.ElapsedMilliseconds >= intervalMilliSec)
							{
								nowProcess_.Refresh();

								// CPU
								{
									double nowCpuUsage = CSystem.getCpuUsageForProcess(nowProcess_, intervalMilliSec);
									peakCPUUsage = double.Max(nowCpuUsage, peakCPUUsage);

									LOG.INFO($"Perf CPU(now: {nowCpuUsage.ToString("0.00")}, peak: {peakCPUUsage.ToString("0.00")})");
								}

								// Memory
								{
									string descMemoryUsage = $"""
														PeakVirtual: {nowProcess_.PeakVirtualMemorySize64.BYTE_TO_GIGABYTE().ToString()} gb,
														PeakPaged: {nowProcess_.PeakPagedMemorySize64.BYTE_TO_MEGABYTE().ToString()} mb, 
														PeakWorkingSet: {nowProcess_.PeakWorkingSet64.BYTE_TO_MEGABYTE().ToString()} mb, 
														GC: {GC.GetTotalMemory(true).BYTE_TO_GIGABYTE().ToString()} gb
														""";
									LOG.INFO($"Perf Memory({descMemoryUsage.Replace(Environment.NewLine, "")})");
								}

								// WorkQueueCount
								{
									LOG.INFO($"Perf Thread(threadCount: {ThreadPool.ThreadCount.ToString()}, completeWork: {ThreadPool.CompletedWorkItemCount.ToString()}, pendingWork: {ThreadPool.PendingWorkItemCount.ToString()})");
								}

								watch.Restart();
							}

							Thread.Sleep(1);
						}
					}, TaskCreationOptions.LongRunning
				).DetectThrowOnDispose();
			}
		}
	}

	internal partial class CApplication
	{
		static Process nowProcess_ = Process.GetCurrentProcess();

		static readonly CPerformanceMeasurer_MT acceptPerformanceMeasurer = new();
		static readonly CPerformanceMeasurer_MT receivePerformanceMeasurer = new();

		private static CSessionConfig sessionConfig_ = new CSessionConfig
		{
			socketOption_ = new CSocketOptionConfig
			{
				lingerOption_ = new LingerOption(true, 0),
			},
			receiveBufferSize_ = 1 << 16,
		};

		private static CAcceptorConfig acceptorConfig_ = new CAcceptorConfig
		{
			socketOption_ = new CSocketOptionConfig { reuseAddress_ = true },
			port_ = 30002,
			backlog_ = 1024,
		};
	}

	internal partial class CApplication
	{
		private static void _onReceived(int bytes)
		{
			receivePerformanceMeasurer.addCount((ulong)bytes);

			bool isCapture = false;
			PerformanceSnapshot snapShot;

			lock (receivePerformanceMeasurer)
			{
				isCapture = receivePerformanceMeasurer.capture(1.SEC_TO_MILLISEC(), out snapShot);
			}

			if (isCapture == true)
			{
				PROFILE.INFO($"Perf Receive(id: {snapShot.id_.ToString()}, now: {snapShot.nowCount_.ToString()}, avg: {snapShot.avgCount_.ToString()}, max: {snapShot.estimatedMaxCount_.ToString()}, total: {snapShot.totalCount_.ToString()})");
			}
		}

		private static void _onAccepted(Socket? socket)
		{
			acceptPerformanceMeasurer.incrementCount();

			{
				PerformanceSnapshot snapshot;
				if (acceptPerformanceMeasurer.capture(1.SEC_TO_MILLISEC(), out snapshot) == true)
				{
					PROFILE.INFO($"Perf Acceptor(id: {snapshot.id_.ToString()}, now: {snapshot.nowCount_.ToString()}, avg: {snapshot.avgCount_.ToString()}, max: {snapshot.estimatedMaxCount_.ToString()}, total: {snapshot.totalCount_.ToString()})");
				}
			}

			// 소켓 멀쩡한지 확인하고,
			if (socket.isValid() == false)
			{
				socket?.Dispose();
				return;
			}

			// 소켓 옵션 조정하고,
			socket?.configureOption(sessionConfig_.socketOption_);

			// ptsoo todo - 암호화 방식 결정

			// 세션 할당해서 붙여주고, 시작
			CTCPSession session = new();
			session.start(socket, sessionConfig_.receiveBufferSize_);
			session.fnOnReceived += _onReceived;
		}

		static void startup()
		{
			ThreadUtil.setThreadName("main");

			launchSubThread(2000);

			PROFILE.init("../cfg/loggersettings_profile.json");

			CNetworkService.instance
				.init()
				.addAcceptor(acceptorConfig_, new fnOnAccepted_t(_onAccepted));

			Minidump.init();

			CNetworkService.instance.launch();
		}

		static void cleanup()
		{
			CNetworkService.instance.stop();
		}

#pragma warning disable CS0162 // 테스트 용도
		static unsafe void Main(string[] args)
		{
			startup();
			cleanup();
		}
#pragma warning restore CS0162
	}
}
