using System.Net;

namespace TSNet
{
	public class CAcceptorConfig
	{
		/// <summary>
		/// 소켓 설정
		/// </summary>
		public CSocketOptionConfig socketOption_;

		/// <summary>
		/// 개방할 IP와 포트
		/// </summary>
		public string ip_ { get; set; } = IPAddress.Any.ToString();
		public int port_ { get; set; } = 0;
		public IPEndPoint? ipEndPoint() => AddressHelper.makeIPEndPoint(ip_, port_);

		/// <summary>
		/// 대기열 크기
		/// </summary>
		public int backlog_ { get; set; } = int.MaxValue;
	}
}
