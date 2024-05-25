namespace TSUtil
{
	/// <summary>
	/// 자료구조 관련 Extension method
	/// </summary>
	public static class CollectionExtensions
	{
		/// <summary>
		/// Array View 관련 Empty 체크
		/// </summary>
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

		public static bool isEmpty<T>(this Memory<T> memory)
		{
			return memory.Length < 1;
		}

		/// <summary>
		/// Array 시작 주소 반환
		/// </summary>
		public static unsafe IntPtr getArrayPtrUnsafe<T>(this Span<T> buffer)
			where T : unmanaged
		{
			fixed (void* pBuffer = &buffer[0])
			{
				IntPtr ptr = (IntPtr)pBuffer;
				return ptr;
			}
		}

		public static unsafe IntPtr getArrayPtrUnsafe<T>(this T[] buffer)
			where T : unmanaged
		{
			fixed (void* pBuffer = buffer)
			{
				IntPtr ptr = (IntPtr)pBuffer;
				return ptr;
			}
		}

		public static unsafe IntPtr getArrayPtrUnsafe<T>(this ArraySegment<T> buffer)
			where T : unmanaged
		{
			return getArrayPtrUnsafe(buffer.AsSpan());
		}

		public static unsafe IntPtr getArrayPtrUnsafe<T>(this Memory<T> buffer)
			where T : unmanaged
		{
			return getArrayPtrUnsafe(buffer.Span);
		}
	}
}
