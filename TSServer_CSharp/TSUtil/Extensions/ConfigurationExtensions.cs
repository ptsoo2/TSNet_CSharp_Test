using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace extensions
{
	/// <summary>
	/// Configuration 관련 Extension method
	/// </summary>
	public static class ConfigurationExtensions
	{
		public static void foreachElements(this IConfigurationRoot configRoot, Action<string, string?> fnIter)
		{
			foreach (var iter in configRoot.AsEnumerable())
			{
				fnIter(iter.Key, iter.Value);
			}
		}

		public static void foreachByPattern(this IConfigurationRoot configRoot, string pattern, RegexOptions regexOption, Action<string, string?> fnIter)
		{
			configRoot.foreachElements(
				(key, value) =>
				{
					if (Regex.IsMatch(key, pattern, regexOption) == false)
						return;

					fnIter(key, value);
				}
			);
		}

		public static List<string> findKeyByPattern(this IConfigurationRoot configRoot, string pattern, RegexOptions regexOption)
		{
			List<string> lstFindKey = new();
			configRoot.foreachByPattern(pattern, regexOption,
				(key, value) =>
				{
					lstFindKey.Add(key);
				}
			);

			return lstFindKey;
		}
	}
}
