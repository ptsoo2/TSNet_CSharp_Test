using ThreadType;

namespace TSUtil
{
	/// <summary>
	/// Number wrapper 기본 클래스
	/// </summary>
	public interface INumberWrapper<T>
	{
		public T value { get; set; }

		public T addCount(T addVal);
		public T subCount(T addVal);
		public T increment();
		public T decrement();
		public T compareExchange(T newValue, T desired);
		public void zeroize();
	}

	public static class CNumberWrapperFactory
	{
		public static INumberWrapper<TValueType>? create<TValueType, TThreadType>()
			where TValueType : unmanaged
			where TThreadType : unmanaged, IThreadType<TThreadType>
		{
			var valueTypeInfo = typeof(TValueType);
			if (valueTypeInfo == typeof(long))
			{
				return (typeof(TThreadType) == typeof(ThreadType.Single))
					? (INumberWrapper<TValueType>)new Int64_t()
					: (INumberWrapper<TValueType>)new Atomic_Int64_t();
			}
			else if (valueTypeInfo == typeof(ulong))
			{
				return (typeof(TThreadType) == typeof(ThreadType.Single))
					? (INumberWrapper<TValueType>)new UInt64_t()
					: (INumberWrapper<TValueType>)new Atomic_UInt64_t();
			}
			else if (valueTypeInfo == typeof(bool))
			{
				return (typeof(TThreadType) == typeof(ThreadType.Single))
					? (INumberWrapper<TValueType>)new Bool_t()
					: (INumberWrapper<TValueType>)new Atomic_Bool_t();
			}

			throw new NotImplementedException();
		}
	}
}
