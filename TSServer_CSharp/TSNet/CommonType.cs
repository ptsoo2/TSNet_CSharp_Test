using System.Net.Sockets;

namespace TSNet
{
	public delegate void fnOnAccepted_t(Socket? client);
	public delegate void fnOnReceived_t(int bytes);
}
