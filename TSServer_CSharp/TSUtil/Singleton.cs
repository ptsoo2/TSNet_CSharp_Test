
namespace TSUtil
{
	public class Singleton<T> where T : new()
	{
		private static Lazy<T> instance_ = new();

		public static T instance => instance_.Value;
	}
}
