using System.Net.Sockets;
using System.Runtime.InteropServices;
using TSUtil;

namespace TSNet
{
	/// <summary>
	/// Task 기반 Receive IO 동작
	/// </summary>
	public class CSocketTaskBasedReceiveOperation : CSocketReceiveOperationBase, IAsyncOperation<int?>
	{
		protected readonly CancellationToken cancellationToken_;

		public CSocketTaskBasedReceiveOperation(Socket socket, int bufferSize)
			: base(socket, bufferSize)
		{
			cancellationToken_ = cancellationToken();
		}

		public override void run()
		{
			int? result = initiate();
			complete(result);
		}

		public int? initiate()
		{
			_initiateInternalAsync().DetectThrowOnDispose();
			return null;
		}

		public void complete(int? result)
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

		/// <summary>
		/// 이벤트 루프로써 receive 동작 반복
		/// </summary>
		protected async Task _initiateInternalAsync()
		{
			while (cancellationToken_.IsCancellationRequested == false)
			{
				Memory<byte> writableBuffer = messageBuffer_.getWritableBuffer().AsMemory<byte>();
				if (writableBuffer.isEmpty() == true)
				{
					LOG.ERROR($"Receive buffer full!!(socket: {socket_?.Handle.ToString()}");
					close();
					break;
				}

				int receiveSize = 0;
				try
				{
					receiveSize = await _recieveAsync(writableBuffer).ConfigureAwait(false);
				}
				catch (SocketException exception)
				{
					// close
					SocketError errorCode = exception.SocketErrorCode;
					LOG.ERROR($"SocketException (message: {exception.Message}, errorCode: {errorCode.toInt().ToString()})");
					break;
				}
				catch (Exception exception)
				{
					// close
					LOG.ERROR($"Exception!!(error: {exception.Message}, socket: {socket_.Handle.ToString()}");
					break;
				}

				complete(receiveSize);
			}

			close();
		}

		/// <summary>
		/// ValueTask 로써 receive 시작
		/// </summary>
		protected async ValueTask<int> _recieveAsync(Memory<byte> buffer)
		{
			return await socket_.ReceiveAsync(buffer, SocketFlags.None, cancellationToken_).ConfigureAwait(false);
		}
	}
}
