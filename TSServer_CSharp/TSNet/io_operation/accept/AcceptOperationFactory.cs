using System.Net.Sockets;

namespace TSNet
{
	public static class CAcceptOperationFactory
	{
		public static CSocketAcceptOperationBase? create<TAcceptOperation>(Socket socket, fnOnAccepted_t fnOnAccepted)
			where TAcceptOperation : CSocketAcceptOperationBase
		{
			object[] acceptServiceParams = new object[] { socket, fnOnAccepted };
			return Activator.CreateInstance(typeof(TAcceptOperation), acceptServiceParams) as CSocketAcceptOperationBase;
		}
	}

	public static class CAcceptorFactory
	{
		public static CAcceptor<TAcceptOperation>? create<TAcceptOperation>(CAcceptorConfig config, fnOnAccepted_t fnOnAccepted)
			where TAcceptOperation : CSocketAcceptOperationBase, new()
		{
			return new CAcceptor<TAcceptOperation>(config, fnOnAccepted);
		}
	}
}
