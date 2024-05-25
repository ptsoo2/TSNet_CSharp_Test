
namespace TSUtil
{
	/// <summary>
	/// 데이터 단위 변환 관련 Extension method
	/// </summary>
	public static class DataUnitExtensions
	{
		public static long BYTE_TO_KILLOBYTE(this long value)
		{
			return (value / 1000);
		}

		public static long KILLOBYTE_TO_MEGABYTE(this long value)
		{
			return (value / 1000);
		}

		public static long MEGABYTE_TO_GIGABYTE(this long value)
		{
			return (value / 1000);
		}

		public static long BYTE_TO_MEGABYTE(this long value)
		{
			return value.BYTE_TO_KILLOBYTE().KILLOBYTE_TO_MEGABYTE();
		}

		public static long BYTE_TO_GIGABYTE(this long value)
		{
			return value.BYTE_TO_KILLOBYTE().KILLOBYTE_TO_MEGABYTE().MEGABYTE_TO_GIGABYTE();
		}
	}
}
