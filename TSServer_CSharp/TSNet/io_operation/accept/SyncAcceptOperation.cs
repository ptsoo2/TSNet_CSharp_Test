using System.Net.Sockets;
using TSUtil;

namespace TSNet
{
	/// <summary>
	/// 동기 Accept 동작
	/// </summary>
	public class CSocketSyncAcceptOperation : CSocketAcceptOperationBase
	{
		public CSocketSyncAcceptOperation(Socket socket, fnOnAccepted_t fnOnAccepted)
			: base(socket, fnOnAccepted)
		{ }

		public override void run()
		{
			CancellationToken token = cancellationToken();

			Task.Factory.StartNew(
				() =>
				{
					while (token.IsCancellationRequested == false)
					{
						Socket? socket = _initiateInternal();
						if (socket == null)
							break;

						fnOnAccepted_(socket);
					};
				}, TaskCreationOptions.LongRunning
			).DetectThrowOnDispose();
		}

		protected Socket? _initiateInternal()
		{
			ArgumentNullException.ThrowIfNull(socket_, "Not opened socket");

			Socket? acceptSocket = null;

			try
			{
				acceptSocket = socket_.Accept();
			}
			catch (Exception exception)
			{
				LOG.ERROR(exception.ToString());
			}

			return acceptSocket;
		}
	}
}
