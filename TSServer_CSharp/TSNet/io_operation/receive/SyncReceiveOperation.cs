using System.Net.Sockets;
using System.Runtime.InteropServices;
using TSUtil;

namespace TSNet
{
	/// <summary>
	/// 동기 Receive IO 동작
	/// </summary>
	public class CSocketSyncReceiveOperation : CSocketReceiveOperationBase
	{
		public CSocketSyncReceiveOperation(Socket socket, int bufferSize)
			: base(socket, bufferSize)
		{ }

		public override void run()
		{
			CancellationToken token = cancellationToken();

			Task.Factory.StartNew(
				() =>
				{
					while (token.IsCancellationRequested == false)
					{
						int? receiveSize = _initiateInternal();
						if (receiveSize == null)
							break;

						_completeInternal(receiveSize);
					};

					close();
				}, TaskCreationOptions.LongRunning
			).DetectThrowOnDispose();
		}

		protected int? _initiateInternal()
		{
			var writableBuffer = messageBuffer_.getWritableBuffer().AsSpan<byte>();
			if (writableBuffer.isEmpty() == true)
			{
				LOG.ERROR($"Receive buffer full!!(socket: {socket_?.Handle.ToString()}");
				return null;
			}

			int receiveSize = 0;
			try
			{
				receiveSize = socket_.Receive(writableBuffer, SocketFlags.None);
			}
			catch (SocketException exception)
			{
				// close
				SocketError errorCode = exception.SocketErrorCode;
				LOG.ERROR($"SocketException (message: {exception.Message}, errorCode: {errorCode.toInt().ToString()})");
				return null;
			}
			catch (Exception exception)
			{
				// close
				LOG.ERROR($"Exception!!(error: {exception.Message}, socket: {socket_.Handle.ToString()}");
				return null;
			}

			return receiveSize;
		}

		protected void _completeInternal(int? result)
		{
			if (result == null)
				return;

			int size = (int)result;
			if (size < 1)
			{
				close();
				return;
			}

			fnOnReceived_?.Invoke(size);

			messageBuffer_.onWriteEnd((int)size);

			while (true)
			{
				ReadOnlySpan<byte> readableBuffer = messageBuffer_.getReadableBuffer();
				if (readableBuffer.isEmpty() == true)
					break;

				if (readableBuffer.Length < MessageHeader.MESSAGE_HEADER_LENGTH)
					break;

				MessageHeader header = MemoryMarshal.Read<MessageHeader>(readableBuffer);
				if (readableBuffer.Length < header.length_)
					break;

				if (header.length_ > messageBuffer_.bufferMaxLength)
				{
					close();
					break;
				}

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
	}
}
