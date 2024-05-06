using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TSUtil;

namespace TSNet
{
	public abstract class CAcceptorBase : CIOHandlerBase
	{
		public static readonly CPerformanceMeasurer_MT performanceMeasurer = new();
		public bool				isValidSocket => (socket_ is not null);

		protected Socket		socket_;
		protected IPEndPoint	ipEndPoint_;
		protected int			backlog_ = int.MaxValue;

		private bool			isDisposed_;

		public CAcceptorBase(string ip, int port, int backlog = int.MaxValue)
		{
			// endpoint 생성
			IPEndPoint? ipEndPoint = AddressHelper.makeIPEndPoint(ip, port);
			ArgumentNullException.ThrowIfNull(ipEndPoint, "Failed to make endPoint");

			ipEndPoint_ = ipEndPoint;
			backlog_ = backlog;

			// socket 생성
			socket_ = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			socket_.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true); // ptsoo todo - 옵션으로 빼자
			socket_.Bind(ipEndPoint_);
		}

		public virtual bool start()
		{
			if (isValidSocket is false)
				return false;

			socket_.Listen(backlog_);

			ThreadUtil.printWithThreadInfo($"Started acceptor({socket_.Handle.ToString()})");

			_processIO();
			return true;
		}

		public virtual void stop()
		{
			ThreadUtil.printWithThreadInfo($"TryDispose => CAcceptorBase | isDisposed: {isDisposed_.ToString()}");
			if (isDisposed_ == true)
				return;

			// socket_.Connected
			// accept socket 은 이것으로 체크할 수 없다. 내부적으로 _isListening 으로 체크하지만 가져올 수 없다.

			if (socket_ is not null)
			{
				socket_.Close();
				socket_ = null!;
			}

			isDisposed_ = true;
			ThreadUtil.printWithThreadInfo($"Disposed => {this.ToString()}");
		}

		public void _stop()
		{
			GC.SuppressFinalize(this);
		}
	}
}
