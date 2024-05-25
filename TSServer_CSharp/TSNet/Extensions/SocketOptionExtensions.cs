using System.Net.Sockets;

namespace TSNet
{
	public static class SocketOptionExtensions
	{
		public static void configureOption(this Socket socket, CSocketOptionConfig config)
		{
			socket
				.noDelay(config.noDelay_)
				.reuseAddress(config.reuseAddress_)
				.sendTimeout(config.sendTimeout_)
				.receiveTimeout(config.receiveTimeout_)
				.sendBufferSize(config.sendBufferSize_)
				.receiveBufferSize(config.receiveBufferSize_)
				.linger(config.lingerOption_)
				.keepAlive(config.keepAliveOption_)
				;
		}

		public static Socket noDelay(this Socket socket, bool? flag)
		{
			if (flag != null)
				socket.NoDelay = (bool)flag;
			return socket;
		}

		public static Socket reuseAddress(this Socket socket, bool? flag)
		{
			if (flag != null)
				socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, (bool)flag);
			return socket;
		}

		public static Socket sendTimeout(this Socket socket, int? timeout)
		{
			if (timeout != null)
				socket.SendTimeout = (int)timeout;
			return socket;
		}

		public static Socket receiveTimeout(this Socket socket, int? timeout)
		{
			if (timeout != null)
				socket.ReceiveTimeout = (int)timeout;
			return socket;
		}

		public static Socket sendBufferSize(this Socket socket, int? bufferSize)
		{
			if (bufferSize != null)
				socket.SendBufferSize = (int)bufferSize;
			return socket;
		}

		public static Socket receiveBufferSize(this Socket socket, int? bufferSize)
		{
			if (bufferSize != null)
				socket.ReceiveBufferSize = (int)bufferSize;
			return socket;
		}

		public static Socket linger(this Socket socket, LingerOption? option)
		{
			if (option != null)
				socket.LingerState = option;
			return socket;
		}

		public static Socket keepAlive(this Socket socket, KeepAliveOption? option)
		{
			if (option != null)
			{
				KeepAliveOption unwrappedOption = (KeepAliveOption)option;
				socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, unwrappedOption.isEnabled_);

				if (unwrappedOption.isEnabled_ == true)
				{
					socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveTime, unwrappedOption.keepAliveTime_);
					socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveInterval, unwrappedOption.keepAliveInterval_);
					socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveRetryCount, unwrappedOption.keepAliveRetryCount_);
				}
			}
			return socket;
		}
	}
}
