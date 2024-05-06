using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TSUtil;

namespace TSNet
{
    public class CAcceptor_Async_EAP : CAcceptorBase
	{
		protected SocketAsyncEventArgs acceptEventArgs_;

		public CAcceptor_Async_EAP(string ip, int port, int backlog = int.MaxValue)
			: base(ip, port, backlog)
		{
			// eventArgs 생성
			acceptEventArgs_ = new();
			acceptEventArgs_.Completed += new EventHandler<SocketAsyncEventArgs>(
				(object? sender, SocketAsyncEventArgs acceptEventArgs)
					=> _onCompletionIO(acceptEventArgs)
			);
		}

		~CAcceptor_Async_EAP()
		{
			ThreadUtil.printWithThreadInfo("dispose!!");
		}

		protected override object?	_generateIO()
		{
			if (isValidSocket is false)
				throw new NullReferenceException("Not opened socket");

			bool isImmediatelyComplete = socket_.AcceptAsync(acceptEventArgs_) == false;
			if (isImmediatelyComplete == false)
				return null;

			// 즉시 완료된 경우
			return acceptEventArgs_;
		}

		protected override void		_onCompletionIO(object? result)
		{
			if (result is null)
				return;

			performanceMeasurer.incrementCount();

			Socket? acceptSocket = acceptEventArgs_.AcceptSocket;
			if (acceptSocket?.Connected is true)
				acceptSocket.Shutdown(SocketShutdown.Both);

			{
				/*
				// https://blog.naver.com/fish19/120104100937
				// acceptEventArgs_ 를 재사용하기 위한 패턴이다.
				// 내부적으로 AcceptAsync => Socket::GetOrCreateAcceptSocket => AcceptEx 의 절차로 진행 된다.
				// AcceptEx 함수는 Proactor pattern 으로 클라이언트 소켓을 생성하여 미리 accept 를 걸게 된다.
				// 클라이언트 소켓을 생성하는 함수는 Socket::GetOrCreateAcceptSocket 이고, acceptEventArgs_.AcceptSocket 를 인자로 전달 받는다.
				// 인자로 전달받은 소켓이 유효하면 그대로 사용하도록 동작한다. 단, accept 에 사용할 소켓인데 이미 연결이 되어있는 상태라면 이치에 맞지 않으므로 throw 를 일으킨다.
				// 따라서 null 로 만들어주어 내부적으로 새로 socket 을 생성하여 AcceptEx 를 올바르게 실행할 수 있도록 null 로 만들어 두어야 하는 것이다.
				*/
				acceptEventArgs_.AcceptSocket = null;
			}

			if (isValidSocket is false)
				return;

			try
			{
				bool isImmediatelyComplete = socket_.AcceptAsync(acceptEventArgs_) == false;
				if (isImmediatelyComplete == true)
				{
					// 즉시 완료된 경우 (ptsoo todo - stack overflow 생각해야겠지, 또한 여기서 socket 이 올바르리라는 보장이 있을까?)
					_onCompletionIO(acceptEventArgs_);
				}
			}
			catch (Exception exception)
			{
				ThreadUtil.printWithThreadInfo(exception.Message);
			}
		}
	}
}
