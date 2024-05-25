namespace TSNet
{
	/// <summary>
	/// ReceiveBuffer
	/// </summary>
	public class CMessageBuffer
	{
		public int bufferMaxLength => buffer_.Length;

		private byte[] buffer_;
		private byte[] subBuffer_;

		private int writeOffset_ = 0;
		private int writeCycleCount_ = 0;

		private int readOffset_ = 0;
		private int readCycleCount_ = 0;

		private int preReadOffset_ = 0;

		private int pendedDataSize_ = 0;

		public CMessageBuffer(int mainBufferSize, int subBufferSize)
		{
			// ptsoo todo - pin flag 조정을 하면서 퍼포먼스 테스트해볼 것
			// SOH, LOH 에 따른 성능 저하 포인트도 확인해보기
			// 자주 할당, 해제가 반복될 수 있는 것의 멤버로 붙여두지 않고,
			// 별도의 POH 영역으로 분리하고, attach 하여 사용하는 방식으로 격리해야할까?
			buffer_ = GC.AllocateUninitializedArray<byte>(mainBufferSize);
			subBuffer_ = GC.AllocateUninitializedArray<byte>(subBufferSize);
		}

		protected int _moveOffset(int preOffset, int size)
		{
			// offset 증가
			int newOffset = preOffset + size;
			if (newOffset < buffer_.Length)
			{
				// 버퍼 안넘으면 유지
				return newOffset;
			}

			// 넘으면 앞으로 돌림
			return newOffset - (buffer_.Length - 1) - 1;
		}

		protected int _increaseCycleCount(int preWriteCycleCount, int preOffset, int size)
		{
			if (preOffset + size < buffer_.Length)
			{
				// 버퍼 안넘었으면 유지
				return preWriteCycleCount;
			}

			// 넘으면 증가
			return preWriteCycleCount + 1;
		}

		public void onWriteEnd(int size)
		{
			if (size < 1)
				throw new ArgumentException("Zero size");

			int writeOffset = writeOffset_;
			int writeCycleCount = writeCycleCount_;

			writeCycleCount_ = _increaseCycleCount(writeCycleCount, writeOffset, size);
			writeOffset_ = _moveOffset(writeOffset, size);

			pendedDataSize_ += size;
		}

		public void onPreReadEnd(int size)
		{
			if (size < 1)
				throw new ArgumentException("Zero size");

			int preReadOffset = preReadOffset_;
			preReadOffset_ = _moveOffset(preReadOffset, size);

			pendedDataSize_ -= size;
		}

		public void onReadEnd(int size)
		{
			if (size < 1)
				throw new ArgumentException("Zero size");

			int readOffset = readOffset_;
			int readCycleCount = readCycleCount_;

			readCycleCount_ = _increaseCycleCount(readCycleCount, readOffset, size);
			readOffset_ = _moveOffset(readOffset, size);
		}

		public ArraySegment<byte> getWritableBuffer()
		{
			int writeOffset = writeOffset_;
			int readOffset = readOffset_;

			int writeCycleCount = writeCycleCount_;
			int readCycleCount = readCycleCount_;

			int writableSize = (writeCycleCount == readCycleCount)
				? ((buffer_.Length - 1) - writeOffset + 1)
				: (readOffset - writeOffset);

			return new ArraySegment<byte>(buffer_, writeOffset, writableSize);
		}

		public ReadOnlySpan<byte> getReadableBuffer()
		{
			int preReadOffset = preReadOffset_;
			int readableSize = pendedDataSize_;

			if (readableSize < 1)
				return ReadOnlySpan<byte>.Empty;

			// 버퍼를 넘는 경우
			if (preReadOffset + readableSize > buffer_.Length)
			{
				int backSize = buffer_.Length - preReadOffset;
				int frontSize = readableSize - backSize;

				Buffer.BlockCopy(buffer_, preReadOffset, subBuffer_, 0, backSize);
				Buffer.BlockCopy(buffer_, 0, subBuffer_, backSize, frontSize);

				return new ReadOnlySpan<byte>(subBuffer_, 0, readableSize);
			}

			return new ReadOnlySpan<byte>(buffer_, preReadOffset, readableSize);
		}
	}
}
