namespace TSUtil
{
	/// <summary>
	/// 시간 변환 관련 Extension method
	/// </summary>
	public static class TimeUnitExtensions
	{
		public static int SEC_TO_MILLISEC(this int value)
		{
			return value * 1000;
		}

		public static int MILLISEC_TO_SEC(this int value)
		{
			return value / 1000;
		}

		public static int SEC_TO_MIN(this int value)
		{
			return value / 60;
		}

		public static int MIN_TO_SEC(this int value)
		{
			return value * 60;
		}
	}
}
