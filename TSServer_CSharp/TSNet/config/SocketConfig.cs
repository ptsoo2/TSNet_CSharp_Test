using System.Net.Sockets;

namespace TSNet
{
	public struct KeepAliveOption
	{
		public bool isEnabled_ = false;
		public int keepAliveTime_ = 0;
		public int keepAliveInterval_ = 0;
		public int keepAliveRetryCount_ = 0;

		public KeepAliveOption()
		{ }
	}

	/// <summary>
	/// 소켓 옵션 조정 용도
	/// </summary>
	public struct CSocketOptionConfig
	{
		public bool? noDelay_ = null;
		public bool? reuseAddress_ = null;

		/// <summary>
		/// 소켓 단 송, 수신 버퍼 크기(Transport Layer)
		/// </summary>
		public int? sendBufferSize_ = null;
		public int? receiveBufferSize_ = null;

		public int? sendTimeout_ = null;
		public int? receiveTimeout_ = null;

		public LingerOption? lingerOption_ = null;
		public KeepAliveOption? keepAliveOption_ = null;

		public CSocketOptionConfig()
		{ }
	}
}
