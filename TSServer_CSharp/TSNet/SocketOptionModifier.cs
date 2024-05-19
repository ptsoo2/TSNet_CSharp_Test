using System.Net.Sockets;
using TSUtil;

namespace TSNet
{
	public struct KeepAliveOption
	{
		public bool isEnabled_ = false;
		public int keepAliveTime_ = 0;
		public int keepAliveInterval_ = 0;
		public int keepAliveRetryCount_ = 0;

		public KeepAliveOption()
		{ }
	}

	public struct CSocketOptionConfig
	{
		public bool? noDelay_ = null;

		public int? sendTimeout_ = null;
		public int? receiveTimeout_ = null;

		public int? sendBufferSize_ = null;
		public int? receiveBufferSize_ = null;

		public LingerOption? lingerOption_ = null;
		public KeepAliveOption? keepAliveOption_ = null;

		public CSocketOptionConfig()
		{ }
	}

	public class CSocketOptionModifier : Singleton<CSocketOptionModifier>
	{
		public void configure(Socket socket, CSocketOptionConfig config)
		{
			noDelay(socket, config.noDelay_);

			sendTimeout(socket, config.sendTimeout_);
			receiveTimeout(socket, config.receiveTimeout_);

			sendBufferSize(socket, config.sendBufferSize_);
			receiveBufferSize(socket, config.receiveBufferSize_);

			linger(socket, config.lingerOption_);
			keepAlive(socket, config.keepAliveOption_);
		}

		public CSocketOptionModifier noDelay(Socket socket, bool? flag)
		{
			if (flag != null)
				socket.NoDelay = (bool)flag;
			return this;
		}

		public CSocketOptionModifier sendTimeout(Socket socket, int? timeout)
		{
			if (timeout != null)
				socket.SendTimeout = (int)timeout;
			return this;
		}

		public CSocketOptionModifier receiveTimeout(Socket socket, int? timeout)
		{
			if (timeout != null)
				socket.ReceiveTimeout = (int)timeout;
			return this;
		}

		public CSocketOptionModifier sendBufferSize(Socket socket, int? bufferSize)
		{
			if (bufferSize != null)
				socket.SendBufferSize = (int)bufferSize;
			return this;
		}

		public CSocketOptionModifier receiveBufferSize(Socket socket, int? bufferSize)
		{
			if (bufferSize != null)
				socket.ReceiveBufferSize = (int)bufferSize;
			return this;
		}

		public CSocketOptionModifier linger(Socket socket, LingerOption? option)
		{
			if (option != null)
				socket.LingerState = option;
			return this;
		}

		public CSocketOptionModifier keepAlive(Socket socket, KeepAliveOption? option)
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
			return this;
		}
	}
}
