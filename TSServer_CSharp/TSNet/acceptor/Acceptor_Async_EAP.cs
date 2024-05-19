﻿using System.Net.Sockets;
using TSUtil;

namespace TSNet
{
	public class AcceptSocketAsyncEventArgs : SocketAsyncEventArgs
	{
		public bool failed() => (SocketError != SocketError.Success);

		public Socket? popSocket()
		{
			// https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.socketasynceventargs.acceptsocket?view=net-5.0
			// SocketAsyncEventArgs 를 재사용하기 위한 패턴이다.
			// 내부적으로 AcceptAsync => Socket::GetOrCreateAcceptSocket => AcceptEx 의 절차로 진행 된다.
			// AcceptEx 함수는 Proactor pattern 으로 클라이언트 소켓을 생성하여 미리 accept 를 걸게 된다.
			// 클라이언트 소켓을 생성하는 함수는 Socket::GetOrCreateAcceptSocket 이고, acceptEventArgs_.AcceptSocket 를 인자로 전달 받는다.
			// 인자로 전달받은 소켓이 유효하면 그대로 사용하도록 동작한다. 단, accept 에 사용할 소켓인데 이미 연결이 되어있는 상태라면 이치에 맞지 않으므로 throw 를 일으킨다.
			// 따라서 null 로 만들어주어 내부적으로 새로 socket 을 생성하여 AcceptEx 를 올바르게 실행할 수 있도록 null 로 만들어 두어야 하는 것이다.
			Socket? ret = this.AcceptSocket;
			this.AcceptSocket = null;
			return ret;
		}
	}

	public class CAcceptor_Async_EAP : CAcceptorBase
	{
		protected AcceptSocketAsyncEventArgs acceptEventArgs_;
		protected EventHandler<SocketAsyncEventArgs> eventHandler_;

		public CAcceptor_Async_EAP(CAcceptorConfig config, fnOnAccepted_t onAccepted)
			: base(config, onAccepted)
		{
			// eventArgs 매핑
			acceptEventArgs_ = new();
			eventHandler_ = (object? sender, SocketAsyncEventArgs acceptEventArgs) =>
			{
				_complete((AcceptSocketAsyncEventArgs)acceptEventArgs);
			};
			acceptEventArgs_.Completed += eventHandler_;
		}

		protected override object? _initiate()
		{
			Socket socket = socket_;
			AcceptSocketAsyncEventArgs acceptEventArgs = acceptEventArgs_;
			bool isImmediatelyComplete = false;

			CancellationTokenSource source = new();
			try
			{
				isImmediatelyComplete = (socket.AcceptAsync(acceptEventArgs) == false);
			}
			catch (ObjectDisposedException)
			{
				// 종료된 경우 나올 수 있음
				stop();
				return null;
			}
			catch (NullReferenceException)
			{
				// 종료된 경우 나올 수 있음
				stop();
				return null;
			}
			catch (SocketException exception)
			{
				LOG.ERROR($"SocketException!!(message: {exception.Message}, errorCode: {exception.ErrorCode.ToString()})");
				stop();
				return null;
			}
			catch (Exception exception)
			{
				LOG.ERROR($"Exception!!(message: {exception.ToString()})");
				stop();
				return null;
			}

			if (isImmediatelyComplete is false)
				return null;

			// 즉시 완료된 경우에는 바로 결과 처리
			return acceptEventArgs_;
		}

		protected sealed override void _complete(object? result)
		{
			if (result is null)
				return;

			AcceptSocketAsyncEventArgs acceptArgs = (AcceptSocketAsyncEventArgs)result;
			Socket? acceptSocket = acceptArgs.popSocket();

			//Console.WriteLine(acceptSocket?.RemoteEndPoint?.ToString());

			if (acceptArgs.failed() == true)
			{
				bool isListenSocketError = false;
				if (isListenSocketError == true)
				{
					// onErrorServer
					// do stop!!
					return;
				}

				// onErrorClient
				//acceptSocket?.Dispose();
				//ThreadUtil.printWithThreadInfo($"SocketException!!(errorCode: {(int)acceptArgs.SocketError}, message: {acceptArgs.SocketError})");
			}
			else if (acceptSocket != null)
			{
				// 옵션 지정하고,
				// 세션 할당받아서 소켓 지정하고, recv 시작
				fnOnAccepted_(acceptSocket);
			}

			// acceptSocket?.Close();
			_runOnce();
		}

		protected override void _stop()
		{
			acceptEventArgs_.Completed -= eventHandler_;
			acceptEventArgs_.Dispose();

			// socket close - 이 아래에 코드가 있으면 안됩니다.
			base._stop();
		}
	}
}
