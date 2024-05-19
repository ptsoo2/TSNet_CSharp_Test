using System.Net.Sockets;
using TSUtil;

namespace TSNet
{
	public class CAcceptor_Async_APM : CAcceptorBase
	{
		protected AsyncCallback callbackAccepted_;

		public CAcceptor_Async_APM(CAcceptorConfig config, fnOnAccepted_t onAccepted)
			: base(config, onAccepted)
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
				LOG.ERROR($"[Listen] SocketException!!(message: {exception.Message}, errorCode: {exception.ErrorCode.ToString()})");
				return;
			}
			catch (Exception exception)
			{
				LOG.ERROR($"[EndAccept] Exception!!(message: {exception.ToString()})");
				return;
			}

			if (socket == null)
				return;

			fnOnAccepted_(socket);
		}
	}
}
