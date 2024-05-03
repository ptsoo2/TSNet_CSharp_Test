using System.Net;
using System.Net.Sockets;
using TSUtil;

namespace TSNet
{
	public class CAcceptor : IDisposable
	{
		public bool						isValidSocket => socket_ is not null;

		public static CPerformanceMeasurer<ThreadType.Multi> performanceMeasurer = new();

		protected Socket				socket_;
		protected IPEndPoint			ipEndPoint_;
		protected int					backlog_ = int.MaxValue;

		protected SocketAsyncEventArgs	acceptEventArgs_;

		private bool					isDisposed_ = false;

		public CAcceptor(string ip, int port, int backlog = int.MaxValue)
		{
			// endpoint 생성
			IPEndPoint? ipEndPoint = AddressHelper.makeEndPoint(ip, port);
			if (ipEndPoint is null)
				throw new Exception();

			ipEndPoint_ = ipEndPoint;
			backlog_ = backlog;

			// eventArgs 생성
			acceptEventArgs_ = new();
			acceptEventArgs_.Completed += new EventHandler<SocketAsyncEventArgs>(
				(object? sender, SocketAsyncEventArgs acceptEventArgs)
					=> _onCompleteAsyncAccept()
			);

			// socket 생성
			socket_ = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

			// ptsoo todo - 옵션으로 빼자
			socket_.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

			socket_.Bind(ipEndPoint_);
		}

		~CAcceptor()
		{
			Dispose(isManualDispose: false);
		}

		public bool startAsync()
		{
			if (isValidSocket is false)
				return false;

			socket_.Listen(backlog_);

			// accept 시작
			_asyncAccept();
			return true;
		}

		public bool startSync()
		{
			if (isValidSocket is false)
				throw new NullReferenceException("Not opened socket");

			socket_.Listen(backlog_);

			try
			{
				Socket acceptSocket = socket_.Accept();
				// Console.WriteLine($"accepted socket({socket.Handle.ToString()})");
				performanceMeasurer.incrementCount();

				// ptsoo todo - 걍 바로 끊는다 -_-
				acceptSocket.Close();
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception.ToString());
			}

			return true;
		}

		protected void _asyncAccept()
		{
			if (isValidSocket is false)
				throw new NullReferenceException("Not opened socket");

			bool isImmediatelyComplete = socket_.AcceptAsync(acceptEventArgs_) == false;
			if (isImmediatelyComplete == true)
			{
				// 즉시 완료된 경우
				_onCompleteAsyncAccept();
			}
		}

		public void test()
		{
			Task.Run(
				async () => 
				{
					Console.WriteLine("begin");
					Socket socket = await socket_.AcceptAsync();
					Console.WriteLine("after");
				}
			);
		}

		protected void _onCompleteAsyncAccept()
		{
			performanceMeasurer.incrementCount();

			Socket? acceptSocket = acceptEventArgs_.AcceptSocket;
			acceptSocket?.Close();  // ptsoo todo - 일단은 테스트를 위해서 걍 바로 끊는다 -_-

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
			// ThreadUtil.printWithThreadInfo("end accept");

			bool isImmediatelyComplete = socket_.AcceptAsync(acceptEventArgs_) == false;
			if (isImmediatelyComplete == true)
			{
				// 즉시 완료된 경우 (ptsoo todo - stack overflow 생각해야겠지, 또한 여기서 socket 이 올바르리라는 보장이 있을까?)
				_onCompleteAsyncAccept();
			}
		}

		protected virtual void Dispose(bool isManualDispose)
		{
			if (isDisposed_ is true)
				return;

			if (isManualDispose is true)
			{
				// TODO: 관리형 상태(관리형 개체)를 삭제합니다.
			}

			socket_?.Close();

			isDisposed_ = true;
		}

		public void Dispose()
		{
			Dispose(isManualDispose: true);
			GC.SuppressFinalize(this);
		}
	}
}
