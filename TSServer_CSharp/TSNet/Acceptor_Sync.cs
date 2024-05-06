using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TSUtil;

namespace TSNet
{
	public class CAcceptor_Sync : CAcceptorBase
	{
		public CAcceptor_Sync(string ip, int port, int backlog = int.MaxValue)
			: base(ip, port, backlog)
		{ }

		protected override void _processIO()
		{
			Task.Factory.StartNew(new Action(_processIOInternal), TaskCreationOptions.LongRunning);
		}

		public void _processIOInternal()
		{
			// ptsoo todo - 일단 무한 루프로 -_-
			while (true)
			{
				base._processIO();
			}
		}

		protected override object? _generateIO()
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

		protected override void _onCompletionIO(object? result)
		{
			if (result is null)
			{
				ThreadUtil.printWithThreadInfo("result is null!!");
				return;
			}

			performanceMeasurer.incrementCount();

			Socket socket = (Socket)result;

			// ptsoo todo - 걍 바로 끊는다 -_-
			socket.Shutdown(SocketShutdown.Both);
		}
	}
}
