
namespace TSNet
{
	public static class CAcceptorFactory
	{
		public static CAcceptorBase? create<TAcceptor>(CAcceptorConfig config, fnOnAccepted_t onAccepted)
			where TAcceptor : CAcceptorBase
		{
			return (CAcceptorBase?)Activator.CreateInstance(typeof(TAcceptor), config, onAccepted);
		}
	}
}
