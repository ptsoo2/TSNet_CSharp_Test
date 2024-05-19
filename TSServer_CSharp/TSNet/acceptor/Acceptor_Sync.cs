using System.Net.Sockets;
using TSUtil;

namespace TSNet
{
	public class CAcceptor_Sync : CAcceptorBase
	{
		public CAcceptor_Sync(CAcceptorConfig config, fnOnAccepted_t onAccepted)
			: base(config, onAccepted)
		{ }

		protected override void _runOnce()
		{
			Task.Factory.StartNew(new Action(_processIOInternal), TaskCreationOptions.LongRunning);
		}

		public void _processIOInternal()
		{
			// ptsoo todo - 일단 무한 루프로 -_-
			while (true)
			{
				base._runOnce();
			}
		}

		protected override object? _initiate()
		{
			ArgumentNullException.ThrowIfNull(socket_, "Not opened socket");

			Socket? acceptSocket = null;

			try
			{
				acceptSocket = socket_.Accept();
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception.ToString());
			}

			return acceptSocket;
		}

		protected override void _complete(object? result)
		{
			if (result is null)
			{
				LOG.ERROR("result is null!!");
				return;
			}

			fnOnAccepted_((Socket)result);
		}
	}
}
