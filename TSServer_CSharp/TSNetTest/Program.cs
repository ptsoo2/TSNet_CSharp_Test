using System.Diagnostics;
using TSNet;
using TSUtil;

namespace TSNetTest
{
	internal class Program
	{
		static void Main(string[] args)
		{
#pragma warning disable CS0162
			ThreadUtil.setThreadName("Main");
			ThreadUtil.printWithThreadInfo("Start of main");

			Latch waitControl = new();

			// launch stop thread
			Task.Run(
				() =>
				{
					while (Console.KeyAvailable == false)
						Thread.Sleep(500);

					waitControl.countdown();
				}
			);

			List<CAcceptor> lstAcceptor = new();
			for (int i = 0; i < 1; ++i)
			{
				CAcceptor acceptor = new("0.0.0.0", 30002);
				acceptor.startAsync();

				lstAcceptor.Add(acceptor);
			}

			Task.Run(
				() =>
				{
					ThreadUtil.setThreadName("Measurer");

					ulong loopCount = 0;

					while (true)
					{
						PerformanceProperties properties;
						if (CAcceptor.performanceMeasurer.update(1000, out properties) == true)
						{
							ThreadUtil.printWithThreadInfo($"[{loopCount.ToString()}]Accepted socket per sec: {properties.nowCount_.ToString()} {properties.totalCount_.ToString()}");
							++loopCount;
						}

						Thread.Sleep(1);
					}
				}
			);

			waitControl.wait();
			ThreadUtil.printWithThreadInfo("End of main");

#pragma warning restore CS0162
		}
	}
}

