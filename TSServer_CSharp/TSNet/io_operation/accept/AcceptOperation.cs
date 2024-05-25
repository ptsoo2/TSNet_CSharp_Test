using System.Net.Sockets;

namespace TSNet
{
	/// <summary>
	/// 소켓 Accept IO 동작 기본 클래스
	/// </summary>
	public abstract class CSocketAcceptOperationBase : CSocketOperationBase
	{
		protected readonly fnOnAccepted_t fnOnAccepted_;

		public CSocketAcceptOperationBase(Socket socket, fnOnAccepted_t fnOnAccepted)
			: base(socket)
		{
			fnOnAccepted_ = fnOnAccepted;
		}
	}
}
