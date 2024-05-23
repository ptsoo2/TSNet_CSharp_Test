
namespace TSUtil
{
	public static class DataUnitExtensions
	{
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
	}
}