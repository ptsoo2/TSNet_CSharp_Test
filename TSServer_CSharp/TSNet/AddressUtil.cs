using System.Net;

namespace TSNet
{
	public class AddressHelper
	{
		public static IPEndPoint? makeIPEndPoint(string ip, int port)
		{
			IPAddress? ipAddress = IPAddress.Parse(ip);
			if (IPAddress.TryParse(ip, out ipAddress) is false)
			{
				Console.WriteLine($"Failed to parse ipaddress({ip})");
				return null;
			}

			IPEndPoint? ret = null;
			try
			{
				ret = new(ipAddress, port);
			}
			catch (Exception exception)
			{
				Console.WriteLine($"Failed to parse ipEndPoint(error: {exception.ToString()}, ip: {ip}, port: {port.ToString()})");
				return null;
			}

			return ret;
		}
	}
}
