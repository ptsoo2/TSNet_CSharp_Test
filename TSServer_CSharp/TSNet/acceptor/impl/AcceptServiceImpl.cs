using System.Net.Sockets;

namespace TSNet
{
	public abstract class CAcceptServiceImpl : COperationBase
	{
		protected readonly Socket socket_;
		protected readonly fnOnAccepted_t onAccepted_;
		private CancellationTokenSource cancellationTokenSource_;

		public CancellationToken cancellationToken() => cancellationTokenSource_.Token;

		public CAcceptServiceImpl(Socket socket, fnOnAccepted_t onAccepted, CancellationTokenSource cancellationTokenSource)
		{
			socket_ = socket;
			onAccepted_ = onAccepted;
			cancellationTokenSource_ = cancellationTokenSource;
		}

		public void start()
		{
			_runOnce();
		}

		public virtual void stop()
		{
			cancellationTokenSource_.Cancel();
		}

		protected override object? _initiate() => throw new NotImplementedException();

		protected override void _complete(object? result) => throw new NotImplementedException();
	}
}
