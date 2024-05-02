using System.Diagnostics;
using System.Net.Sockets;
using TSUtil;

namespace TSClient
{
	internal class Program
	{
		static void Main(string[] args)
		{
			for (int i = 0; i < 3; ++i)
			{
				List<Socket> lstSocket = new List<Socket>();
				Test.Bench(
					() =>
					{
						for (int i = 0; i < 10; ++i)
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
