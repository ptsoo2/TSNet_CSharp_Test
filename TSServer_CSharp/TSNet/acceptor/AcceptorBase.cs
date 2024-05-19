using System.Net;
using System.Net.Sockets;
using TSUtil;

namespace TSNet
{
	public abstract class CAcceptorBase : Operation
	{
		protected CAcceptorConfig config_ { get; }
		protected Socket socket_ { get; private set; }
		protected fnOnAccepted_t fnOnAccepted_;

		private CSharedCounter_Bool isClosed_ { get; } = new(false);

		public CAcceptorBase(CAcceptorConfig config, fnOnAccepted_t onAccepted)
		{
			config_ = config;
			fnOnAccepted_ += onAccepted;

			IPEndPoint? ipEndPoint = config_.ipEndPoint();
			ArgumentNullException.ThrowIfNull(ipEndPoint, "Failed to make endPoint");

			// socket 생성
			socket_ = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			socket_.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true); // ptsoo todo - 옵션으로 빼자
			socket_.Bind(ipEndPoint);
		}

		public void start()
		{
			ArgumentNullException.ThrowIfNull(socket_, "Socket is null");

			socket_.Listen(config_.backlog);
			LOG.INFO($"Started acceptor(endPoint: {config_.ipEndPoint()}, socket: {socket_.Handle.ToString()})");

			_runOnce();
		}

		public void stop()
		{
			if (isClosed_.compareExchange(true, false) == true)
			{
				LOG.WARNING("Already closed acceptor");
				return;
			}

			_stop();
		}

		protected virtual void _stop()
		{
			try
			{
				// socket_.Connected
				// accept socket 은 이것으로 체크할 수 없다. 내부적으로 _isListening 으로 체크하지만 가져올 수 없다.
				socket_.Close();
				socket_ = null!;
			}
			catch (SocketException exception)
			{
				LOG.ERROR($"SocketException!!(message: {exception.Message}, errorCode: {exception.ErrorCode.ToString()})");
			}
			catch (Exception exception)
			{
				LOG.ERROR($"Exception: {exception.ToString()}");
			}

			LOG.INFO($"Stop Acceptor({this.ToString()})");
		}
	}
}
