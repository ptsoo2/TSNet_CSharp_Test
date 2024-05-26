namespace TSUtil
{
	/// <summary>
	/// 숫자 유형 관련 Extension method
	/// </summary>
	public static class NumberExtensions
	{
		public static bool convertToBool(this long value)
		{
			return (value != 0);
		}

		public static long convertToLong(this bool value)
		{
			return (value == true) ? 1 : 0;
		}
	}
}
