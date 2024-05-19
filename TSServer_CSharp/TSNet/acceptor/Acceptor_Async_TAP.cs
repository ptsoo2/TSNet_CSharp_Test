using System.Net.Sockets;
using TSUtil;

namespace TSNet
{
	public class CAcceptor_Async_TAP : CAcceptorBase
	{
		private CancellationTokenSource cancellationTokenSource_ = new CancellationTokenSource();

		public CAcceptor_Async_TAP(CAcceptorConfig config, fnOnAccepted_t onAccepted)
			: base(config, onAccepted)
		{ }

		protected override object? _initiate()
		{
			_generateIOInternalAsync().DetectThrowOnDispose();
			return null;
		}

		protected async Task _generateIOInternalAsync()
		{
			ArgumentNullException.ThrowIfNull(socket_);

			Socket? socket = null;

			while (cancellationTokenSource_.IsCancellationRequested == false)
			{
				try
				{
					// Task 를 사용하는 경우 내부적으로 try ~ catch 에 의해 실행되기 때문에
					// 명시적으로 사용자 코드에서 try ~ catch 하지않는한 인지할 수 없다.
					// 따라서 전체를 try ~ catch 블록으로 감싸서 예외를 처리한다.

					// 또한 try ~ catch 블록은 가능한 하나만 다루도록 하자. 여러 개를 사용하면 예외 식별이 흐려진다.
					socket = await socket_
						.AcceptAsync(cancellationTokenSource_.Token)
						.ConfigureAwait(false)
						;
				}
				catch (SocketException exception)
				{
					// pass
					SocketError errorCode = (SocketError)exception.ErrorCode;
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
					LOG.ERROR($"OperationCanceledException (message: {exception.Message})");
					break;
				}
				catch (Exception exception)
				{
					// stop
					LOG.ERROR($"Exception (message: {exception.Message})");
					break;
				}

				_complete(socket);
				socket = null;
			}

			stop();
		}

		protected override void _complete(object? result)
		{
			if (result is null)
				return;

			Socket socket = (Socket)result;
			fnOnAccepted_(socket);
		}

		protected override void _stop()
		{
			cancellationTokenSource_.Cancel();

			// socket close - 이 아래에 코드가 있으면 안됩니다.
			base._stop();
		}
	}
}
