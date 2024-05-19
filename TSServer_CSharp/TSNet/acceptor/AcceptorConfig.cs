using System.Net;

namespace TSNet
{
	public class CAcceptorConfig
	{
		public string ip { get; set; } = IPAddress.Any.ToString();
		public int port { get; set; } = 0;
		public int backlog { get; set; } = int.MaxValue;

		public IPEndPoint? ipEndPoint() => AddressHelper.makeIPEndPoint(ip, port);
	}
}
