using System.Net;
using System.Net.Sockets;
using TSUtil;

namespace TSNet
{
	public class CAcceptor<TAcceptService>
		where TAcceptService : CAcceptServiceImpl
	{
		private CSharedCounter_Bool isClosed_ { get; } = new(false);

		protected CAcceptorConfig config_ { get; }
		protected Socket socket_ { get; private set; }
		protected CAcceptServiceImpl impl_;

		public CAcceptor(CAcceptorConfig config, fnOnAccepted_t onAccepted)
		{
			config_ = config;

			IPEndPoint? ipEndPoint = config_.ipEndPoint();
			ArgumentNullException.ThrowIfNull(ipEndPoint, "Failed to make endPoint");

			// socket 생성
			socket_ = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			socket_.configureOption(config.socketOption_);

			socket_.Bind(ipEndPoint);

			// Impl stop => Acceptor stop 의 순서를 맞춰주기 위해 등록 후 넘겨준다.
			// Cancel 콜 시점에 등록의 역순으로 호출된다.
			CancellationTokenSource cancellationTokenSource = new();
			cancellationTokenSource.Token.Register(new Action(_stop));

			impl_ = CAcceptServiceFactory.create<TAcceptService>(socket_, onAccepted, cancellationTokenSource)!;
		}

		public void start()
		{
			ArgumentNullException.ThrowIfNull(socket_, "Socket is null");
			LOG.INFO($"Started acceptor(endPoint: {config_.ipEndPoint()}, socket: {socket_.Handle.ToString()})");

			socket_.Listen(config_.backlog_);
			impl_.start();
		}

		public void stop()
		{
			impl_.stop();
		}

		protected void _stop()
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
