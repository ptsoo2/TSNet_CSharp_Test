using System.Runtime.CompilerServices;

namespace TSUtil
{
	/// <summary>
	/// NumberWrapper Multi Thread 버전
	/// </summary>
	public class Atomic_Int64_t : INumberWrapper<long>
	{
		protected long value_ = 0;
		public long value
		{
			get => Interlocked.Read(ref value_);
			set { Interlocked.Exchange(ref value_, value); }
		}

		public long addCount(long addVal)
		{
			return Interlocked.Add(ref value_, addVal);
		}

		public long subCount(long subVal)
		{
			return addCount(-1 * subVal);
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
			return Interlocked.CompareExchange(ref value_, newValue, desired);
		}

		public void zeroize() { value = 0; }
	}

	public class Atomic_UInt64_t : INumberWrapper<ulong>
	{
		protected ulong value_ = 0;
		public ulong value
		{
			get => Interlocked.Read(ref value_);
			set { Interlocked.Exchange(ref value_, value); }
		}

		public ulong addCount(ulong addVal)
		{
			return Interlocked.Add(ref value_, addVal);
		}

		public ulong subCount(ulong subVal)
		{
			long sub = -1 * Unsafe.As<ulong, long>(ref subVal);
			return (ulong)Interlocked.Add(ref Unsafe.As<ulong, long>(ref value_), sub);
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
			return Interlocked.CompareExchange(ref value_, newValue, desired);
		}

		public void zeroize() { value = 0; }
	}

	public class Atomic_Bool_t : INumberWrapper<bool>
	{
		protected long value_ = 0;
		public bool value
		{
			get => Interlocked.Read(ref value_).convertToBool();
			set { Interlocked.Exchange(ref value_, value.convertToLong()).convertToBool(); }
		}

		public bool compareExchange(bool newValue, bool desired)
		{
			return Interlocked.CompareExchange(ref value_, newValue.convertToLong(), desired.convertToLong()).convertToBool();
		}

		public void zeroize() { value = false; }

		public bool addCount(bool addVal) => throw new NotImplementedException();
		public bool subCount(bool subVal) => throw new NotImplementedException();
		public bool increment() => throw new NotImplementedException();
		public bool decrement() => throw new NotImplementedException();
	}
}
