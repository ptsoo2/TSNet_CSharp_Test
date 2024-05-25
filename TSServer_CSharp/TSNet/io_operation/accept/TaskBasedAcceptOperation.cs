using System.Net.Sockets;
using TSUtil;

namespace TSNet
{
	/// <summary>
	/// Task 기반 Accept 동작
	/// </summary>
	public class CSocketTaskBasedAcceptOperation : CSocketAcceptOperationBase, IAsyncOperation<Socket?>
	{
		protected readonly CancellationToken cancellationToken_;

		public CSocketTaskBasedAcceptOperation(Socket socket, fnOnAccepted_t onAccepted)
			: base(socket, onAccepted)
		{
			cancellationToken_ = cancellationToken();
		}

		public override void run()
		{
			Socket? socket = initiate();
			complete(socket);
		}

		public Socket? initiate()
		{
			_initiateInternalAsync(cancellationToken_).DetectThrowOnDispose();
			return null;
		}

		public void complete(Socket? result)
		{
			if (result is null)
				return;

			fnOnAccepted_(result);
		}

		protected async Task _initiateInternalAsync(CancellationToken cancellationToken)
		{
			ArgumentNullException.ThrowIfNull(socket_);

			Socket? socket = null;

			while (cancellationToken.IsCancellationRequested == false)
			{
				try
				{
					// Task 를 사용하는 경우 내부적으로 try ~ catch 에 의해 실행되기 때문에
					// 명시적으로 사용자 코드에서 try ~ catch 하지않는한 인지할 수 없다.
					// 따라서 전체를 try ~ catch 블록으로 감싸서 예외를 처리한다.

					// 또한 try ~ catch 블록은 가능한 하나만 다루도록 하자. 여러 개를 사용하면 예외 식별이 흐려진다.
					socket = await socket_.AcceptAsync(cancellationToken).ConfigureAwait(false);
				}
				catch (SocketException exception)
				{
					// pass
					SocketError errorCode = exception.SocketErrorCode;
					if (errorCode != SocketError.ConnectionReset)
						LOG.ERROR($"SocketException (message: {exception.Message}, errorCode: {errorCode.toInt().ToString()})");
				}
				catch (ObjectDisposedException exception)
				{
					// stop
					LOG.ERROR($"ObjectDisposedException (message: {exception.Message})");
					break;
				}
				catch (NullReferenceException exception)
				{
					// stop
					LOG.ERROR($"NullReferenceException (message: {exception.Message})");
					break;
				}
				catch (OperationCanceledException exception)
				{
					// stop
					LOG.WARNING($"OperationCanceledException (message: {exception.Message})");
					break;
				}
				catch (Exception exception)
				{
					// stop
					LOG.ERROR($"Exception (message: {exception.Message})");
					break;
				}

				complete(socket);
				socket = null;
			}

			close();
		}
	}
}
