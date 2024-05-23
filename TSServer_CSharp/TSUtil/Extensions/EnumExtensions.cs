
namespace TSUtil
{
	public static class EnumExtensions
	{
		public static int toInt<T>(this T value) where T : IConvertible//enum
		{
			if (typeof(T).IsEnum == false)
			{
				throw new ArgumentException("T must be an enumerated type");
			}

			return (int)(IConvertible)value;
		}
	}
}
