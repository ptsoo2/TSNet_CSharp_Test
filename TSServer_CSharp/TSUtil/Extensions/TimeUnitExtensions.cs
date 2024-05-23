
namespace TSUtil
{
	public static class TimeUnitExtensions
	{
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
