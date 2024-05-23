using System.Net.Sockets;
using TSUtil;

namespace TSNet
{
	public class CSyncAcceptService : CAcceptServiceImpl
	{
		public CSyncAcceptService(Socket socket, fnOnAccepted_t onAccepted, CancellationTokenSource cancellationTokenSource)
			: base(socket, onAccepted, cancellationTokenSource)
		{ }

		protected override object? _initiate()
		{
			CancellationToken token = cancellationToken();

			Task.Factory.StartNew(
				() =>
				{
					// ptsoo todo - 일단은 동기 acceptor 는 걍 thread 따로 빼서 처리
					while (token.IsCancellationRequested == false)
					{
						_complete(_initiateInternal());
					};
				}
			, TaskCreationOptions.LongRunning);
			return null;
		}

		protected override void _complete(object? result)
		{
			if (result == null)
				return;

			onAccepted_((Socket)result);
		}

		protected object? _initiateInternal()
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
