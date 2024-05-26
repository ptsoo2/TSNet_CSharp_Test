using System.Net.Sockets;
using TSUtil;

namespace TSNet
{
	public class CTCPSession
	{
		public long handle => (socket_?.Handle.ToInt64() ?? -1);

		protected Socket? socket_ = null;
		protected CSocketReceiveOperationBase? receiveOperation_ = null;

		public event fnOnReceived_t fnOnReceived
		{
			add
			{
				ArgumentNullException.ThrowIfNull(receiveOperation_);
				receiveOperation_.fnOnReceived += value;
			}
			remove
			{
				ArgumentNullException.ThrowIfNull(receiveOperation_);
				receiveOperation_.fnOnReceived -= value;
			}
		}

		public void start(Socket? socket, int bufferSize)
		{
			if (socket.isValid() == false)
			{
				socket?.Dispose();
				return;
			}

			// 멤버 세팅
			socket_ = socket;

			// Operation close => Acceptor close 의 순서를 맞춰주기 위해 선 등록 해준다.
			// Cancel 콜 시점에 등록의 역순으로 호출된다.
			receiveOperation_ = new CSocketTaskBasedReceiveOperation(socket_!, bufferSize);
			receiveOperation_.cancellationToken().Register(new Action(this._close));

			// recv 시작
			receiveOperation_.run();
		}

		public void close()
		{
			receiveOperation_?.close();
		}

		protected void _close()
		{
			try
			{
				socket_?.Close();
				socket_ = null!;
			}
			catch (SocketException exception)
			{
				// pass
				SocketError errorCode = exception.SocketErrorCode;
				LOG.ERROR($"SocketException (message: {exception.Message}, errorCode: {errorCode.toInt().ToString()})");
			}
		}
	}
}
