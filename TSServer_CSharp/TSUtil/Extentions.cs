
namespace TSUtil
{
	public static class Extensions
	{
		public static void DoNothing(this Task task)
		{}

		public static void DoNothing(this ValueTask task)
		{}

		// data unit
		public static long BYTE_TO_KILLOBYTE(this long value)
		{
			return DataUnit.BYTE_TO_KILLOBYTE(value);
		}
		public static long KILLOBYTE_TO_MEGABYTE(this long value)
		{
			return DataUnit.KILLOBYTE_TO_MEGABYTE(value);
		}
		public static long MEGABYTE_TO_GIGABYTE(this long value)
		{
			return DataUnit.MEGABYTE_TO_GIGABYTE(value);
		}
		public static long BYTE_TO_MEGABYTE(this long value)
		{
			return DataUnit.BYTE_TO_MEGABYTE(value);
		}
		public static long BYTE_TO_GIGABYTE(this long value)
		{
			return DataUnit.BYTE_TO_GIGABYTE(value);
		}

		// time unit
		public static int SEC_TO_MILLISEC(this int value)
		{
			return TimeUnit.SEC_TO_MILLISEC(value);
		}
		public static int MILLISEC_TO_SEC(this int value)
		{
			return TimeUnit.MILLISEC_TO_SEC(value);
		}
		public static int SEC_TO_MIN(this int value)
		{
			return TimeUnit.SEC_TO_MIN(value);
		}
		public static int MIN_TO_SEC(this int value)
		{
			return TimeUnit.MIN_TO_SEC(value);
		}
	}
}
