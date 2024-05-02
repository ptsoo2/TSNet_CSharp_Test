using System.Diagnostics;
using TSNet;
using TSUtil;

namespace TSNetTest
{
	public abstract class ICounter
	{
		protected ulong counter_ = 0;

		public abstract void increase(ulong val);
	}

	public class CInterlockedCounter : ICounter
	{
		public override void increase(ulong val)
		{
			
		}
	}

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
					Stopwatch watch = new();

					ulong loopCount = 0;

					watch.Start();
					while (true)
					{
						if (watch.ElapsedMilliseconds > 1000)
						{
							uint counter = Interlocked.Exchange(ref CAcceptor.acceptCounter, 0);
							ThreadUtil.printWithThreadInfo($"[{loopCount.ToString()}]Accepted socket per sec: {counter.ToString()} {CAcceptor.totalAcceptCounter}");

							++loopCount;

							watch.Restart();
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

