#define _ACCEPT_TEST_

using System.Net.Sockets;
using TSNet;
using TSUtil;

namespace TSClient
{
	internal class Program
	{
		static void Main(string[] args)
		{
			CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

			Task.Factory.StartNew(
				() =>
				{
					while (Console.KeyAvailable == false)
						Thread.Sleep(500);

					cancellationTokenSource.Cancel();
				}, TaskCreationOptions.LongRunning
			).DetectThrowOnDispose();

#if _ACCEPT_TEST_
			CancellationToken token = cancellationTokenSource.Token;
			while (token.IsCancellationRequested == false)
			{
				Test.Bench(
					() =>
					{
						for (int i = 0; i < 1000; ++i)
						{
							if (token.IsCancellationRequested == true)
								break;

							Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
							socket.linger(new LingerOption(true, 0));

							try
							{
								socket.Connect("172.30.1.62", 30002);
								//Console.WriteLine(socket.LocalEndPoint?.ToString());
							}
							catch (Exception exception)
							{
								Console.WriteLine($"Failed to connect{exception.ToString()}");
							}

							socket.Close();
						}
					}
				);
				Thread.Sleep(1);
			}
#else // _ACCEPT_TEST_
			Atomic_UInt64_t sharedCounter = new();

			while (true)
			{
				int SOCKET_COUNT = 1000;
				int SEND_COUNT = 2500;

				List<Socket> lstSocket = new();
				Test.Bench(() =>
					{
						for (int i = 0; i < SOCKET_COUNT; ++i)
						{
							Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
							try
							{
								socket.Connect("172.30.1.62", 30002);
							}
							catch (Exception exception)
							{
								Console.WriteLine(exception.Message);
								break;
							}

							socket.sendBufferSize(int.MaxValue);
							lstSocket.Add(socket);
						}
					}, desc: "Connecting..."
				);

				if (lstSocket.Count != SOCKET_COUNT)
					continue;

				Console.WriteLine("All Connected");

				bool isConnected = true;
				while (isConnected == true)
				{
					Test.Bench(
						() =>
						{
							int totalLength = 0;
							List<byte[]> lstSendBuffer = new();
							List<ArraySegment<byte>> lstArraySegment = new();

							Parallel.ForEach(Enumerable.Range(0, SEND_COUNT),
								(idx) =>
								{
									{
										byte[] sendBuffer;
										TestMessage testMessage = new($"Hello, World!{(sharedCounter.increment()).ToString()}");
										testMessage.writeTo(out sendBuffer);

										lock (lstSendBuffer)
										{
											lstSendBuffer.Add(sendBuffer);
											lstArraySegment.Add(new ArraySegment<byte>(sendBuffer));
											totalLength += sendBuffer.Length;
										}
									}
								}
							);

							Console.WriteLine($"Total length: {totalLength.ToString()} bytes...");

							Parallel.ForEach(
								lstSocket,
								new ParallelOptions { MaxDegreeOfParallelism = 4 },
								iter =>
								{
									try
									{
										iter.Send(lstArraySegment);
									}
									catch (Exception exception)
									{
										Console.WriteLine(exception.Message);
										isConnected = false;
										return;
									}
								}
							);
						}, desc: $"Sending..."
					);
				}
#endif //_ACCEPT_TEST_
		}
	}
}

