using System.Net.Sockets;
using TSUtil;

namespace TSNet
{
	public class CAPMBasedAcceptService : CAcceptServiceImpl
	{
		protected AsyncCallback callbackAccepted_;

		public CAPMBasedAcceptService(Socket socket, fnOnAccepted_t onAccepted, CancellationTokenSource cancellationTokenSource)
			: base(socket, onAccepted, cancellationTokenSource)
		{
			callbackAccepted_ = new AsyncCallback(_complete);
		}

		protected override object? _initiate()
		{
			IAsyncResult? result = null;
			try
			{
				result = socket_.BeginAccept(callbackAccepted_, null);
			}
			catch (Exception exception)
			{
				LOG.ERROR($"[BeginAccept] Exception!!(message: {exception.ToString()})");
			}

			if (result is null)
				return null;

			if (result.CompletedSynchronously is false)
				return null;

			// 동기적으로 종료되었다면 바로 반환한다.
			return result;
		}

		protected override void _complete(object? result)
		{
			if (result is null)
				return;

			_onCompletionIOInternal((IAsyncResult)result);
			_runOnce();
		}

		protected void _onCompletionIOInternal(IAsyncResult result)
		{
			Socket? socket = null;
			try
			{
				socket = socket_.EndAccept(result);
			}
			catch (SocketException exception)
			{
				SocketError errorCode = exception.SocketErrorCode;
				if (errorCode != SocketError.ConnectionReset)
					LOG.ERROR($"[Listen] SocketException!!(message: {exception.Message}, errorCode: {exception.ErrorCode.ToString()})");
				return;
			}
			catch (Exception exception)
			{
				LOG.ERROR($"[EndAccept] Exception!!(message: {exception.ToString()})");
				return;
			}

			if (socket != null)
				onAccepted_(socket);
		}
	}
}
