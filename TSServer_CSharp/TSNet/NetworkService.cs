using TSUtil;

namespace TSNet
{
	using CAcceptor = CAcceptor<CTaskBasedAcceptService>;

	public class CNetworkService : Singleton<CNetworkService>
	{
		public bool running => (latchStop_.isEnd() == false);

		protected Latch latchStop_ = new Latch();
		protected List<CAcceptor> lstAcceptor_ = new();

		public CNetworkService init()
		{
			LOG.init("loggersettings.json");
			return this;
		}

		public void launch()
		{
			for (int i = 0; i < lstAcceptor_.Count; ++i)
				lstAcceptor_[i].start();

			latchStop_.wait();
		}

		public void stop()
		{
			for (int i = 0; i < lstAcceptor_.Count; ++i)
				lstAcceptor_[i].stop();

			LOG.close();
			latchStop_.countdown();
		}

		public CNetworkService addAcceptor(CAcceptorConfig config, fnOnAccepted_t onAccepted)
		{
			CAcceptor acceptor = new(config, onAccepted);
			lstAcceptor_.Add(acceptor);
			return this;
		}
	}
}
