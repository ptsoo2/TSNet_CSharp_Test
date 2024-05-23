using System.Net.Sockets;
using System.Runtime.InteropServices;
using TSUtil;

namespace TSNet
{
	public class CTCPSession
	{
		public long handle => (socket_?.Handle.ToInt64() ?? -1);

		protected Socket socket_ = null!;
		protected CMessageBuffer messageBuffer_ = null!;

		public void start(Socket socket, int bufferSize)
		{
			messageBuffer_ = new CMessageBuffer(bufferSize, bufferSize);

			socket_ = socket;
			LOG.INFO($"Started Session(socket: {handle.ToString()})");

			_beginRecv().DetectThrowOnDispose();
		}

		protected async Task _beginRecv()
		{
			while (true)
			{
				ArraySegment<byte> writableBuffer = messageBuffer_.getWritableBuffer();
				if (writableBuffer.isEmpty() == true)
				{
					LOG.ERROR($"Recv buffer full!!(socket: {handle.ToString()}");
					_close();
					return;
				}

				int recvSize = 0;
				try
				{
					recvSize = await socket_?.ReceiveAsync(writableBuffer, SocketFlags.None)!;
				}
				catch (Exception exception)
				{
					LOG.ERROR($"Exception!!(error: {exception.Message}, socket: {handle.ToString()}");
					_close();
					return;
				}

				_endRecv(recvSize);
			}
		}

		protected void _endRecv(int size)
		{
			if (size < 1)
			{
				LOG.ERROR($"Size is zero!!(socket: {handle.ToString()}, size: {size.ToString()})");
				_close();
				return;
			}

			messageBuffer_.onWriteEnd(size);

			while (true)
			{
				ReadOnlySpan<byte> readableBuffer = messageBuffer_.getReadableBuffer();
				if (readableBuffer.isEmpty() == true)
					break;

				if (readableBuffer.Length < MessageHeader.PACKET_HEADER_LENGTH)
					break;

				MessageHeader header = MemoryMarshal.Read<MessageHeader>(readableBuffer);
				if (readableBuffer.Length < header.length_)
					break;

				// 읽을 수 있는 만큼으로 줄여주고,
				readableBuffer = readableBuffer.Slice(0, header.length_);

				messageBuffer_.onPreReadEnd(readableBuffer.Length);

				{
					TestMessage message = new TestMessage();
					message.readFrom(ref readableBuffer);
					// LOG.DEBUG($"message: `{message.message ?? "unknown"}`, size: {readableBuffer.Length.ToString()}");
				}

				messageBuffer_.onReadEnd(readableBuffer.Length);
			}
		}

		protected void _close()
		{
			try
			{
				socket_?.Close();
				socket_ = null!;
			}
			catch (SocketException exception)
			{
				// pass
				SocketError errorCode = exception.SocketErrorCode;
				LOG.ERROR($"SocketException (message: {exception.Message}, errorCode: {errorCode.toInt().ToString()})");
			}
		}
	}
}
