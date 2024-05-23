using System.Net;
using TSUtil;

namespace TSNet
{
	public class AddressHelper
	{
		public static IPEndPoint? makeIPEndPoint(string ip, int port)
		{
			IPAddress? ipAddress = IPAddress.Parse(ip);
			if (IPAddress.TryParse(ip, out ipAddress) is false)
			{
				LOG.ERROR($"Failed to parse ipaddress({ip})");
				return null;
			}

			IPEndPoint? ret = null;
			try
			{
				ret = new(ipAddress, port);
			}
			catch (Exception exception)
			{
				LOG.ERROR($"Failed to parse ipEndPoint(error: {exception.ToString()}, ip: {ip}, port: {port.ToString()})");
				return null;
			}

			return ret;
		}
	}
}
