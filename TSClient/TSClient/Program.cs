using System.Net.Sockets;
using TSUtil;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace TSClient
{
	internal class Program
	{
		static void Main(string[] args)
		{
			while (true)
			{
				List<Socket> list = new List<Socket>();
				for (int i = 0; i < 1000; ++i)
				{
					Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					socket.LingerState = new LingerOption(true, 0);

					try
					{
						socket.Connect("172.30.1.62", 30002);
						//Console.WriteLine(socket.LocalEndPoint?.ToString());
					}
					catch (Exception exception)
					{
						Console.WriteLine($"Failed to connect{exception.ToString()}");
						socket.Close();
						continue;
					}

					socket.Close();
				}
				Thread.Sleep(1);
			}
		}
	}
}
