using System.Net.Sockets;

namespace TSNet
{
	/// <summary>
	/// 소켓 IO 동작 기본 클래스
	/// </summary>
	public abstract class CSocketOperationBase : IOperation
	{
		protected readonly Socket socket_;
		protected readonly CancellationTokenSource cancellationTokenSource_ = new CancellationTokenSource();

		public CancellationToken cancellationToken() => cancellationTokenSource_.Token;

		public CSocketOperationBase(Socket socket)
		{
			socket_ = socket;
		}

		public virtual void close()
		{
			cancellationTokenSource_.Cancel();
		}

		public abstract void run();
	}
}
