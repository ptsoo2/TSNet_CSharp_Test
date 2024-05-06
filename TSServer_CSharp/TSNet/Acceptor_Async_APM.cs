using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TSNet
{
	public class CAcceptor_Async_APM : CAcceptorBase
	{
		protected AsyncCallback callbackAccepted_;

		public CAcceptor_Async_APM(string ip, int port, int backlog = int.MaxValue)
			: base(ip, port, backlog)
		{
			callbackAccepted_ = new AsyncCallback(_onCompletionIOInternal);
		}

		protected override object? _generateIO()
		{
			var result = socket_.BeginAccept(callbackAccepted_, null);
			if (result is null)
				return null;

			if (result.CompletedSynchronously is false)
				return null;

			// 동기적으로 종료되었다면 바로 반환한다.
			return result;
		}

		protected override void _onCompletionIO(object? result)
		{
			if (result is null)
				return;

			_onCompletionIOInternal((IAsyncResult)result);
		}

		protected void _onCompletionIOInternal(IAsyncResult result)
		{
			performanceMeasurer.incrementCount();

			Socket socket = socket_.EndAccept(result);
			socket.Shutdown(SocketShutdown.Both);

			_processIO();
		}
	}
}
