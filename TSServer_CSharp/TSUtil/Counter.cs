using System.Diagnostics;
using System.Runtime.CompilerServices;
using ThreadType;

// Unsafe.As 는 아래와 같이 동작함
// ptsoo todo - 현재 메모리 침범할 수 있는 문제가 있음. 수정 필요
//static unsafe TTo MyUnsafeAs<TFrom, TTo>(TFrom rhs)
//{
//	return *(TTo*)(&rhs);
//}

//static unsafe ref TTo MyUnsafeAs2<TFrom, TTo>(ref TFrom rhs)
//{
//	fixed (void* pRhs = &rhs)
//	{
//		return ref *(TTo*)pRhs;
//	}
//}

namespace TSUtil
{
	/// <summary>
	/// 간단한 Counter class (ST, MT 지원)
	/// </summary>
	/// <todo>
	/// 테스트 필요
	/// return type 수정 필요
	/// </todo>
	/// <typeparam name="T"></typeparam>
	public interface ICounter<T>
	{
		public T value { get; set; }

		public T addCount(T addVal);
		public T increment();
		public T decrement();
		public T compareExchange(T newValue, T desired);
		public void zeroize();
	}

	public abstract class CCounterBase<T> : ICounter<T>
		where T : unmanaged
	{
		public CCounterBase(T initializeValue = default) { value = initializeValue; }

		protected long INCREMENT_VALUE = 1;

		protected T value_;
		public abstract T value { get; set; }

		public abstract T addCount(T addVal);
		public abstract T increment();
		public abstract T decrement();
		public abstract T compareExchange(T newValue, T desired);
		public abstract void zeroize();
	}

	public class CCounter<T> : CCounterBase<T>
		where T : unmanaged
	{
		public CCounter(T initializeValue = default) : base(initializeValue)
		{ }

		public override T value
		{
			get => value_;
			set { value_ = value; }
		}

		public override unsafe T addCount(T addVal)
		{
			fixed (void* pValue = &value_)
			{
				if (value_ is sbyte) Unsafe.AsRef<sbyte>(pValue) += Unsafe.As<T, sbyte>(ref addVal);
				else if (value_ is byte) Unsafe.AsRef<byte>(pValue) += Unsafe.As<T, byte>(ref addVal);
				else if (value_ is short) Unsafe.AsRef<short>(pValue) += Unsafe.As<T, short>(ref addVal);
				else if (value_ is ushort) Unsafe.AsRef<ushort>(pValue) += Unsafe.As<T, ushort>(ref addVal);
				else if (value_ is int) Unsafe.AsRef<int>(pValue) += Unsafe.As<T, int>(ref addVal);
				else if (value_ is uint) Unsafe.AsRef<uint>(pValue) += Unsafe.As<T, uint>(ref addVal);
				else if (value_ is long) Unsafe.AsRef<long>(pValue) += Unsafe.As<T, long>(ref addVal);
				else if (value_ is ulong) Unsafe.AsRef<ulong>(pValue) += Unsafe.As<T, ulong>(ref addVal);
				else throw new NotImplementedException("Not implementation");
			}

			return value_;
		}

		public override unsafe T increment()
		{
			return addCount(Unsafe.As<long, T>(ref INCREMENT_VALUE));
		}

		public override unsafe T decrement()
		{
			fixed (void* pValue = &value_)
			{
				if (value_ is sbyte) Unsafe.AsRef<sbyte>(pValue) -= 1;
				else if (value_ is byte) Unsafe.AsRef<byte>(pValue) -= 1;
				else if (value_ is short) Unsafe.AsRef<short>(pValue) -= 1;
				else if (value_ is ushort) Unsafe.AsRef<ushort>(pValue) -= 1;
				else if (value_ is int) Unsafe.AsRef<int>(pValue) -= 1;
				else if (value_ is uint) Unsafe.AsRef<uint>(pValue) -= 1;
				else if (value_ is long) Unsafe.AsRef<long>(pValue) -= 1;
				else if (value_ is ulong) Unsafe.AsRef<ulong>(pValue) -= 1;
				else throw new NotImplementedException("Not implementation");
			}

			return value_;
		}

		public override unsafe T compareExchange(T newValue, T desired)
		{
			T origin = value_;
			fixed (void* pValue = &value_)
			{
				if (value_ is sbyte)
				{
					if (Unsafe.As<T, sbyte>(ref value_) == *(sbyte*)(&desired)) value_ = newValue;
				}
				else if (value_ is byte)
				{
					if (Unsafe.As<T, byte>(ref value_) == *(byte*)(&desired)) value_ = newValue;
				}
				else if (value_ is short)
				{
					if (Unsafe.As<T, short>(ref value_) == *(short*)(&desired)) value_ = newValue;
				}
				else if (value_ is ushort)
				{
					if (Unsafe.As<T, ushort>(ref value_) == *(ushort*)(&desired)) value_ = newValue;
				}
				else if (value_ is int)
				{
					if (Unsafe.As<T, int>(ref value_) == *(int*)(&desired)) value_ = newValue;
				}
				else if (value_ is uint)
				{
					if (Unsafe.As<T, uint>(ref value_) == *(uint*)(&desired)) value_ = newValue;
				}
				else if (value_ is long)
				{
					if (Unsafe.As<T, long>(ref value_) == *(long*)(&desired)) value_ = newValue;
				}
				else if (value_ is ulong)
				{
					if (Unsafe.As<T, ulong>(ref value_) == *(ulong*)(&desired)) value_ = newValue;
				}
				else
				{
					throw new NotImplementedException("Not implementation");
				}
			}
			return origin;
		}

		public override unsafe void zeroize() { value = default; }
	}

	public class CSharedCounter<T> : CCounterBase<T>
		where T : unmanaged
	{
		public CSharedCounter(T initializeValue = default) : base(initializeValue)
		{ }

#pragma warning disable CS8500
		public override unsafe T value
		{
			get
			{
				bool isSigned = false;
				(long, ulong) pairNumber = (0, 0);

				fixed (void* pValue = &value_)
				{
					isSigned = (value_ is sbyte)
						|| (value_ is short)
						|| (value_ is int)
						|| (value_ is long)
						|| (value_ is bool)
						;

					if (isSigned == true) pairNumber.Item1 = Interlocked.Read(ref Unsafe.AsRef<long>(pValue));
					else pairNumber.Item2 = Interlocked.Read(ref Unsafe.AsRef<ulong>(pValue));
				}

				return (isSigned == true)
					? Unsafe.As<long, T>(ref pairNumber.Item1)
					: Unsafe.As<ulong, T>(ref pairNumber.Item2);
			}
			set
			{
				fixed (void* pValue = &value_)
				{
					if (value_ is sbyte) Interlocked.Exchange(ref Unsafe.AsRef<int>(pValue), Unsafe.As<T, int>(ref value));
					else if (value_ is byte) Interlocked.Exchange(ref Unsafe.AsRef<uint>(pValue), Unsafe.As<T, uint>(ref value));
					else if (value_ is short) Interlocked.Exchange(ref Unsafe.AsRef<int>(pValue), Unsafe.As<T, int>(ref value));
					else if (value_ is ushort) Interlocked.Exchange(ref Unsafe.AsRef<uint>(pValue), Unsafe.As<T, uint>(ref value));
					else if (value_ is int) Interlocked.Exchange(ref Unsafe.AsRef<int>(pValue), Unsafe.As<T, int>(ref value));
					else if (value_ is uint) Interlocked.Exchange(ref Unsafe.AsRef<uint>(pValue), Unsafe.As<T, uint>(ref value));
					else if (value_ is long) Interlocked.Exchange(ref Unsafe.AsRef<long>(pValue), Unsafe.As<T, long>(ref value));
					else if (value_ is ulong) Interlocked.Exchange(ref Unsafe.AsRef<ulong>(pValue), Unsafe.As<T, ulong>(ref value));
					else if (value_ is bool) Interlocked.Exchange(ref Unsafe.AsRef<int>(pValue), Unsafe.As<T, int>(ref value)); // int 와 동일하게 취급
					else throw new NotImplementedException("Not implementation");
				}
			}
		}

		public override unsafe T addCount(T addVal)
		{
			fixed (void* pValue = &value_)
			{
				if (addVal is sbyte)
				{
					var a = Interlocked.Add(ref Unsafe.AsRef<int>(pValue), Unsafe.As<T, sbyte>(ref addVal));
					return Unsafe.As<int, T>(ref a);
				}
				else if (addVal is byte)
				{
					var a = Interlocked.Add(ref Unsafe.AsRef<uint>(pValue), Unsafe.As<T, byte>(ref addVal));
					return Unsafe.As<uint, T>(ref a);
				}
				else if (addVal is short)
				{
					var a = Interlocked.Add(ref Unsafe.AsRef<int>(pValue), Unsafe.As<T, short>(ref addVal));
					return Unsafe.As<int, T>(ref a);
				}
				else if (addVal is ushort)
				{
					var a = Interlocked.Add(ref Unsafe.AsRef<uint>(pValue), Unsafe.As<T, ushort>(ref addVal));
					return Unsafe.As<uint, T>(ref a);
				}
				else if (addVal is int)
				{
					var a = Interlocked.Add(ref Unsafe.AsRef<int>(pValue), Unsafe.As<T, int>(ref addVal));
					return Unsafe.As<int, T>(ref a);
				}
				else if (addVal is uint)
				{
					var a = Interlocked.Add(ref Unsafe.AsRef<uint>(pValue), Unsafe.As<T, uint>(ref addVal));
					return Unsafe.As<uint, T>(ref a);
				}
				else if (addVal is long)
				{
					var a = Interlocked.Add(ref Unsafe.AsRef<long>(pValue), Unsafe.As<T, long>(ref addVal));
					return Unsafe.As<long, T>(ref a);
				}
				else if (addVal is ulong)
				{
					var a = Interlocked.Add(ref Unsafe.AsRef<ulong>(pValue), Unsafe.As<T, ulong>(ref addVal));
					return Unsafe.As<ulong, T>(ref a);
				}
			}

			throw new NotImplementedException("Not implementation");
		}

		public override unsafe T increment()
		{
			return addCount(Unsafe.As<long, T>(ref INCREMENT_VALUE));
		}

		public override unsafe T decrement()
		{
			fixed (void* pValue = &value_)
			{
				if (value_ is sbyte)
				{
					var a = Interlocked.Decrement(ref Unsafe.AsRef<int>(pValue));
					return Unsafe.As<int, T>(ref a);
				}
				else if (value_ is byte)
				{
					var a = Interlocked.Decrement(ref Unsafe.AsRef<uint>(pValue));
					return Unsafe.As<uint, T>(ref a);
				}
				else if (value_ is short)
				{
					var a = Interlocked.Decrement(ref Unsafe.AsRef<int>(pValue));
					return Unsafe.As<int, T>(ref a);
				}
				else if (value_ is ushort)
				{
					var a = Interlocked.Decrement(ref Unsafe.AsRef<uint>(pValue));
					return Unsafe.As<uint, T>(ref a);
				}
				else if (value_ is int)
				{
					var a = Interlocked.Decrement(ref Unsafe.AsRef<int>(pValue));
					return Unsafe.As<int, T>(ref a);
				}
				else if (value_ is uint)
				{
					var a = Interlocked.Decrement(ref Unsafe.AsRef<uint>(pValue));
					return Unsafe.As<uint, T>(ref a);
				}
				else if (value_ is long)
				{
					var a = Interlocked.Decrement(ref Unsafe.AsRef<long>(pValue));
					return Unsafe.As<long, T>(ref a);
				}
				else if (value_ is ulong)
				{
					var a = Interlocked.Decrement(ref Unsafe.AsRef<ulong>(pValue));
					return Unsafe.As<ulong, T>(ref a);
				}
			}

			throw new NotImplementedException("Not implementation");
		}

		public override unsafe T compareExchange(T newValue, T desired)
		{
			fixed (void* pValue = &value_)
			{
				if (value_ is sbyte)
				{
					var a = Interlocked.CompareExchange(ref Unsafe.AsRef<int>(pValue), Unsafe.As<T, int>(ref newValue), Unsafe.As<T, int>(ref desired));
					return Unsafe.As<int, T>(ref a);
				}
				else if (value_ is byte)
				{
					var a = Interlocked.CompareExchange(ref Unsafe.AsRef<uint>(pValue), Unsafe.As<T, uint>(ref newValue), Unsafe.As<T, uint>(ref desired));
					return Unsafe.As<uint, T>(ref a);
				}
				else if (value_ is short)
				{
					var a = Interlocked.CompareExchange(ref Unsafe.AsRef<int>(pValue), Unsafe.As<T, int>(ref newValue), Unsafe.As<T, int>(ref desired));
					return Unsafe.As<int, T>(ref a);
				}
				else if (value_ is ushort)
				{
					var a = Interlocked.CompareExchange(ref Unsafe.AsRef<uint>(pValue), Unsafe.As<T, uint>(ref newValue), Unsafe.As<T, uint>(ref desired));
					return Unsafe.As<uint, T>(ref a);
				}
				else if (value_ is int)
				{
					var a = Interlocked.CompareExchange(ref Unsafe.AsRef<int>(pValue), Unsafe.As<T, int>(ref newValue), Unsafe.As<T, int>(ref desired));
					return Unsafe.As<int, T>(ref a);
				}
				else if (value_ is uint)
				{
					var a = Interlocked.CompareExchange(ref Unsafe.AsRef<uint>(pValue), Unsafe.As<T, uint>(ref newValue), Unsafe.As<T, uint>(ref desired));
					return Unsafe.As<uint, T>(ref a);
				}
				else if (value_ is long)
				{
					var a = Interlocked.CompareExchange(ref Unsafe.AsRef<long>(pValue), Unsafe.As<T, long>(ref newValue), Unsafe.As<T, long>(ref desired));
					return Unsafe.As<long, T>(ref a);
				}
				else if (value_ is ulong)
				{
					var a = Interlocked.CompareExchange(ref Unsafe.AsRef<ulong>(pValue), Unsafe.As<T, ulong>(ref newValue), Unsafe.As<T, ulong>(ref desired));
					return Unsafe.As<ulong, T>(ref a);
				}
				else if (value_ is bool)
				{
					var a = Interlocked.CompareExchange(ref Unsafe.AsRef<int>(pValue), Unsafe.As<T, int>(ref newValue), Unsafe.As<T, int>(ref desired));
					return Unsafe.As<int, T>(ref a);
				}
				else throw new NotImplementedException("Not implementation");
			}
		}

		public override unsafe void zeroize() { value = default; }
#pragma warning restore CS8500
	}

	/// <summary>
	/// 스레드 사용 유형에 따라서 Counter 할당
	/// </summary>
	public static class CounterFactory
	{
		public static ICounter<TCounterType>? create<TCounterType, TThreadType>()
			where TCounterType : unmanaged
			where TThreadType : unmanaged, IThreadType<TThreadType>
		{
			return typeof(TThreadType) == typeof(ThreadType.Multi)
				? new CSharedCounter<TCounterType>()
				: new CCounter<TCounterType>();
		}
	}

	static class CounterTest
	{
		public static void test_counter_int()
		{
			/*
			get, set
			increment, decrement
			경계쪽 테스트 (overflow, underflow)
			*/

			CCounter<int> test = new();
			Debug.Assert(test.value == 0);

			// set, get
			test.value = 1;
			Debug.Assert(test.value == 1);

			// increment
			test.increment();
			Debug.Assert(test.value == 2);

			test.increment();
			Debug.Assert(test.value == 3);

			test.increment();
			Debug.Assert(test.value == 4);

			test.increment();
			Debug.Assert(test.value == 5);

			// decrement
			test.decrement();
			Debug.Assert(test.value == 4);

			test.decrement();
			Debug.Assert(test.value == 3);

			test.decrement();
			Debug.Assert(test.value == 2);

			test.decrement();
			Debug.Assert(test.value == 1);

			test.decrement();
			Debug.Assert(test.value == 0);

			test.decrement();
			Debug.Assert(test.value == -1);
		}
	}
}

namespace TSUtil
{
	// st version
	public sealed class CCounter_Int8 : CCounter<sbyte> { public CCounter_Int8(sbyte value = default) : base(value) { } }
	public sealed class CCounter_UInt8 : CCounter<byte> { public CCounter_UInt8(byte value = default) : base(value) { } }
	public sealed class CCounter_Int16 : CCounter<short> { public CCounter_Int16(short value = default) : base(value) { } }
	public sealed class CCounter_UInt16 : CCounter<ushort> { public CCounter_UInt16(ushort value = default) : base(value) { } }
	public sealed class CCounter_Int32 : CCounter<int> { public CCounter_Int32(int value = default) : base(value) { } }
	public sealed class CCounter_UInt32 : CCounter<uint> { public CCounter_UInt32(uint value = default) : base(value) { } }
	public sealed class CCounter_Int64 : CCounter<long> { public CCounter_Int64(long value = default) : base(value) { } }
	public sealed class CCounter_UInt64 : CCounter<ulong> { public CCounter_UInt64(ulong value = default) : base(value) { } }
	public sealed class CCounter_Bool : CCounter<bool> { public CCounter_Bool(bool value = default) : base(value) { } }

	// mt version
	public sealed class CSharedCounter_Int8 : CSharedCounter<sbyte> { public CSharedCounter_Int8(sbyte value = default) : base(value) { } }
	public sealed class CSharedCounter_UInt8 : CSharedCounter<byte> { public CSharedCounter_UInt8(byte value = default) : base(value) { } }
	public sealed class CSharedCounter_Int16 : CSharedCounter<short> { public CSharedCounter_Int16(short value = default) : base(value) { } }
	public sealed class CSharedCounter_UInt16 : CSharedCounter<ushort> { public CSharedCounter_UInt16(ushort value = default) : base(value) { } }
	public sealed class CSharedCounter_Int32 : CSharedCounter<int> { public CSharedCounter_Int32(int value = default) : base(value) { } }
	public sealed class CSharedCounter_UInt32 : CSharedCounter<uint> { public CSharedCounter_UInt32(uint value = default) : base(value) { } }
	public sealed class CSharedCounter_Int64 : CSharedCounter<long> { public CSharedCounter_Int64(long value = default) : base(value) { } }
	public sealed class CSharedCounter_UInt64 : CSharedCounter<ulong> { public CSharedCounter_UInt64(ulong value = default) : base(value) { } }
	public sealed class CSharedCounter_Bool : CSharedCounter<bool> { public CSharedCounter_Bool(bool value = default) : base(value) { } }
}
