using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Channels;
using TSUtil;

namespace TSNet
{
	public class CAcceptor : CAcceptorBase
	{
		public CAcceptor(string ip, int port, int backlog = int.MaxValue)
			: base(ip, port, backlog)
		{
		}

		public override bool start()
		{
			return base.start();
		}

		protected override object? _generateIO()
		{
			ThreadUtil.printWithThreadInfo("_generateIO");
			_generateIOInternalAsync().DoNothing();
			return null;
		}

		protected async Task _generateIOInternalAsync()
		{
			ArgumentNullException.ThrowIfNull(socket_);

			try
			{
				// try~catch 가 있어야 잡힌다.
				Socket? socket = await socket_.AcceptAsync().ConfigureAwait(false);
				_onCompletionIO(socket);
			}
			catch (SocketException exception)
			{
				ThreadUtil.printWithThreadInfo($"SocketException!!(message: {exception.Message}, errorCode: {exception.ErrorCode.ToString()})");

				var socketError = (SocketError)exception.ErrorCode;
				if ((socketError == SocketError.NotConnected)
					|| (socketError == SocketError.OperationAborted))
				{
					// 이미 종료되었기에 추가로 해줄 작업이 없다.
					socket_ = null!;
					_stop();
					return;
				}
			}
			catch (InvalidOperationException exception)
			{
				ThreadUtil.printWithThreadInfo($"InvalidOperationException: {exception.ToString()}");
			}
			catch (Exception exception)
			{
				ThreadUtil.printWithThreadInfo($"Exception: {exception.ToString()}");
			}
		}

		protected override void _onCompletionIO(object? result)
		{
			ThreadUtil.printWithThreadInfo("_onCompletionIO");
			if (result is null)
				return;

			if (result is not Socket)
			{
				ThreadUtil.printWithThreadInfo("Not a socket type");
				return;
			}

			performanceMeasurer.incrementCount();

			Socket socket = (Socket)result;

			// ptsoo todo - 임시로
			socket.Shutdown(SocketShutdown.Both);

			_generateIO();
		}
	}
}
