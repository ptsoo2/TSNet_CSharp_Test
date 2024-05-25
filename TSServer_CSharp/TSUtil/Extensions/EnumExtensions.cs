namespace TSUtil
{
	/// <summary>
	/// Enum 관련 Extension method
	/// </summary>
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
