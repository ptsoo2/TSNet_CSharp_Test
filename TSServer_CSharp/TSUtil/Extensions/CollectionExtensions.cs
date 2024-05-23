namespace TSUtil
{
	public static class CollectionExtensions
	{
		public static bool isEmpty<T>(this ArraySegment<T> array)
		{
			return array.Count < 1;
		}

		public static bool isEmpty<T>(this ReadOnlySpan<T> span)
		{
			return span.Length < 1;
		}

		public static bool isEmpty<T>(this Span<T> span)
		{
			return span.Length < 1;
		}

		public static unsafe IntPtr getArrayPtrUnsafe<T>(this Span<T> buffer)
			where T : unmanaged
		{
			fixed (void* pBuffer = &buffer[0])
			{
				IntPtr ptr = (IntPtr)pBuffer;
				return ptr;
			}
		}

		static unsafe IntPtr getArrayPtrUnsafe<T>(this T[] buffer)
			where T : unmanaged
		{
			fixed (void* pBuffer = buffer)
			{
				IntPtr ptr = (IntPtr)pBuffer;
				return ptr;
			}
		}

		static unsafe IntPtr getArrayPtrUnsafe<T>(this ArraySegment<T> buffer)
			where T : unmanaged
		{
			return getArrayPtrUnsafe(buffer.AsSpan());
		}

		static unsafe IntPtr getArrayPtrUnsafe<T>(this Memory<T> buffer)
			where T : unmanaged
		{
			return getArrayPtrUnsafe(buffer.Span);
		}
	}
}
