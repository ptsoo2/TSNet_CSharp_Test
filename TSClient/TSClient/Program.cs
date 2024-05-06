using System.Net.Sockets;
using TSUtil;

namespace TSClient
{
	internal class Program
	{
		static void Main(string[] args)
		{
			for (int i = 0; i < 1; ++i)
			{
				List<Socket> lstSocket = new List<Socket>();
				Test.Bench(
					() =>
					{
						for (int i = 0; i < 1000; ++i)
						{
							Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

							try
							{
								socket.Connect("172.30.1.62", 30002);
							}
							catch (Exception exception)
							{
								Console.WriteLine($"Failed to connect{exception.ToString()}");
								socket.Close();
								continue;
							}

							lstSocket.Add(socket);
						}
					}, 1
				);

				foreach (var item in lstSocket)
				{
					item.Close();
				}
			}
		}
	}
}
//namespace TSClient
//{
//	public class MyDisposable : IDisposable
//	{
//		private bool isDisposed_ = false;

//		~MyDisposable()
//		{
//			Dispose(false);
//		}

//		public void Dispose()
//		{
//			Dispose(true);
//			GC.SuppressFinalize(this);
//		}

//		protected virtual void Dispose(bool isManualDispose)
//		{
//			if (isDisposed_ == true)
//			{
//				Console.WriteLine("Already disposed!");
//				return;
//			}

//			if (isManualDispose == true)
//			{
//				Console.WriteLine("Disposing by Dispose()");
//			}
//			else
//			{
//				Console.WriteLine("Disposing by ~MyDisposable");
//			}

//			Console.WriteLine("Disposed");
//			isDisposed_ = true;
//		}
//	}

//	public class MyDisposable2 : MyDisposable
//	{
//		~MyDisposable2()
//		{
//			Console.WriteLine("Disposing by ~MyDisposable2");
//		}
//	}

//	public class Program
//	{
//		private static void disposeTest()
//		{
//			new MyDisposable2();
//		}

//		static void Main(string[] args)
//		{
//			Console.WriteLine("[main] Constructing");
//			disposeTest();
//			GC.Collect();
//			GC.WaitForPendingFinalizers();
//			Console.ReadKey();
//		}
//	}
//}
