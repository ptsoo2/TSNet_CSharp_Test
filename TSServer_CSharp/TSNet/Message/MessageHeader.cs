﻿using System.Runtime.InteropServices;
using System.Text;

namespace TSNet
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct MessageHeader
	{
		public static readonly ushort PACKET_HEADER_LENGTH = (ushort)Marshal.SizeOf<MessageHeader>();

		public readonly ushort length_;
		// ptsoo todo type

		public MessageHeader(int length)
		{
			if (length_ > ushort.MaxValue)
				throw new ArgumentOutOfRangeException($"length is too large(length: {length.ToString()})");

			length_ = (ushort)length;
		}
	}


	// ptsoo todo - test
	public struct TestMessage
	{
		public MessageHeader header_ { get; private set; }
		private byte[] message_ = null!;

		public string message => Encoding.UTF8.GetString(message_);

		public TestMessage() { }
		public TestMessage(string message)
		{
			header_ = new MessageHeader(MessageHeader.PACKET_HEADER_LENGTH + message.Length);
			message_ = Encoding.UTF8.GetBytes(message, 0, message.Length);
		}

		public Span<byte> writeTo(out byte[] outBuffer)
		{
			outBuffer = new byte[header_.length_];

			MemoryMarshal.Write(outBuffer.AsSpan<byte>(), header_);
			message_.AsSpan().CopyTo(outBuffer.AsSpan<byte>(MessageHeader.PACKET_HEADER_LENGTH));

			return outBuffer.AsSpan<byte>();
		}

		public void readFrom(ref ReadOnlySpan<byte> fromBuffer)
		{
			header_ = MemoryMarshal.Read<MessageHeader>(fromBuffer);

			message_ = new byte[header_.length_ - MessageHeader.PACKET_HEADER_LENGTH];
			fromBuffer.Slice(MessageHeader.PACKET_HEADER_LENGTH, message_.Length).CopyTo(message_);
		}
	}
}
