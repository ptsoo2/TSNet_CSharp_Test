using System.Net.Sockets;
using TSUtil;

namespace TSNet
{
	/// <summary>
	/// APM 기반 Accept 동작
	/// </summary>
	public class CSocketAPMBasedAcceptOperation : CSocketAcceptOperationBase, IAsyncOperation<IAsyncResult?>
	{
		protected AsyncCallback callbackAccepted_;

		public CSocketAPMBasedAcceptOperation(Socket socket, fnOnAccepted_t onAccepted)
			: base(socket, onAccepted)
		{
			callbackAccepted_ = new AsyncCallback(complete);
		}

		public override void run()
		{
			IAsyncResult? result = initiate();
			complete(result);
		}

		public IAsyncResult? initiate()
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

		public void complete(IAsyncResult? result)
		{
			if (result is null)
				return;

			_onCompletionIOInternal(result);
			run();
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
				fnOnAccepted_(socket);
		}
	}
}
