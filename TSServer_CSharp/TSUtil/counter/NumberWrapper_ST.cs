namespace TSUtil
{
	/// <summary>
	/// NumberWrapper Single Thread 버전
	/// </summary>
	public class Int64_t : INumberWrapper<long>
	{
		protected long value_ = 0;
		public long value
		{
			get => value_;
			set { value_ = value; }
		}

		public long addCount(long addVal)
		{
			value_ += addVal;
			return value_;
		}

		public long subCount(long subVal)
		{
			value_ -= subVal;
			return value_;
		}

		public long increment()
		{
			return addCount(1);
		}

		public long decrement()
		{
			return subCount(1);
		}

		public long compareExchange(long newValue, long desired)
		{
			long origin = value_;
			if (value_ == desired) value_ = newValue;
			return origin;
		}

		public void zeroize() { value = 0; }
	}

	public class UInt64_t : INumberWrapper<ulong>
	{
		protected ulong value_ = 0;
		public ulong value
		{
			get => value_;
			set { value_ = value; }
		}

		public ulong addCount(ulong addVal)
		{
			value_ += addVal;
			return value_;
		}

		public ulong subCount(ulong subVal)
		{
			value_ -= subVal;
			return value_;
		}

		public ulong increment()
		{
			return addCount(1);
		}

		public ulong decrement()
		{
			return subCount(1);
		}

		public ulong compareExchange(ulong newValue, ulong desired)
		{
			ulong origin = value_;
			if (value_ == desired) value_ = newValue;
			return origin;
		}

		public void zeroize() { value = 0; }
	}

	public class Bool_t : INumberWrapper<bool>
	{
		protected long value_ = 0;
		public bool value
		{
			get => value_.convertToBool();
			set { value_ = value.convertToLong(); }
		}

		public bool compareExchange(bool newValue, bool desired)
		{
			bool origin = value;
			if (value == desired) value_ = newValue.convertToLong();
			return origin;
		}

		public void zeroize() { value = false; }

		public bool addCount(bool addVal) => throw new NotImplementedException();
		public bool subCount(bool subVal) => throw new NotImplementedException();
		public bool increment() => throw new NotImplementedException();
		public bool decrement() => throw new NotImplementedException();
	}
}
