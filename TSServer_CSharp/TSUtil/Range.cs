
namespace TSUtil
{
	public static class Range
	{
		public static TEnum? GetMaxValue<TEnum>()
			where TEnum : Enum
		{
			return Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Max();
		}
	}
}
