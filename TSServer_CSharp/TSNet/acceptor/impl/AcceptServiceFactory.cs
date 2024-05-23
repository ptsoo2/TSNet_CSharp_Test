using System.Net.Sockets;

namespace TSNet
{
	public static class CAcceptorFactory
	{
		public static CAcceptor<TAcceptService>? create<TAcceptService>(CAcceptorConfig config, fnOnAccepted_t fnOnAccepted)
			where TAcceptService : CAcceptServiceImpl, new()
		{
			return new CAcceptor<TAcceptService>(config, fnOnAccepted);
		}
	}

	public static class CAcceptServiceFactory
	{
		public static CAcceptServiceImpl? create<TAcceptService>(Socket socket, fnOnAccepted_t fnOnAccepted, CancellationTokenSource cancellationTokenSource)
			where TAcceptService : CAcceptServiceImpl
		{
			object[] acceptServiceParams = new object[] { socket, fnOnAccepted, cancellationTokenSource };
			return Activator.CreateInstance(typeof(TAcceptService), acceptServiceParams) as CAcceptServiceImpl;
		}
	}
}
