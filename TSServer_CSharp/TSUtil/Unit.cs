namespace TSUtil
{
	public static class DataUnit
	{
		public static long BYTE_TO_KILLOBYTE(long value)
		{
			return (value / 1000);
		}
		public static long KILLOBYTE_TO_MEGABYTE(long value)
		{
			return (value / 1000);
		}
		public static long MEGABYTE_TO_GIGABYTE(long value)
		{
			return (value / 1000);
		}
		public static long BYTE_TO_MEGABYTE(long value)
		{
			return KILLOBYTE_TO_MEGABYTE(BYTE_TO_KILLOBYTE(value));
		}
		public static long BYTE_TO_GIGABYTE(long value)
		{
			return MEGABYTE_TO_GIGABYTE(KILLOBYTE_TO_MEGABYTE(BYTE_TO_KILLOBYTE(value)));
		}
	}

	public static class TimeUnit
	{
		public static int SEC_TO_MILLISEC(int value)
		{
			return value * 1000;
		}
		public static int MILLISEC_TO_SEC(int value)
		{
			return value / 1000;
		}
		public static int SEC_TO_MIN(int value)
		{
			return value / 60;
		}
		public static int MIN_TO_SEC(int value)
		{
			return value * 60;
		}
	}
}
