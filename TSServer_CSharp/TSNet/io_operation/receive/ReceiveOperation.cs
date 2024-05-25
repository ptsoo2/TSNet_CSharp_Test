using System.Net.Sockets;

namespace TSNet
{
	/// <summary>
	/// 소켓 Receive IO 동작 기본 클래스
	/// </summary>
	public abstract class CSocketReceiveOperationBase : CSocketOperationBase
	{
		protected readonly CMessageBuffer messageBuffer_;

		protected fnOnReceived_t? fnOnReceived_ { get; private set; }
		public event fnOnReceived_t fnOnReceived
		{
			add => fnOnReceived_ += value;
			remove => fnOnReceived_ -= value;
		}

		public CSocketReceiveOperationBase(Socket socket, int bufferSize)
			: base(socket)
		{
			messageBuffer_ = new CMessageBuffer(bufferSize, bufferSize);
		}
	}
}
