using System.Net.Sockets;

namespace TSNet
{
	public static class SocketExtensions
	{
		public static long handleToLong(this Socket socket)
		{
			return socket.Handle.ToInt64();
		}

		public static bool isValid(this Socket? socket)
		{
			if (socket == null)
				return false;

			return (socket.Connected == true);
		}
	}
}
