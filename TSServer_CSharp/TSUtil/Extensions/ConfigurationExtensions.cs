using Microsoft.Extensions.Configuration;
using TSUtil;

namespace extensions
{
	/// <summary>
	/// Configuration 관련 Extension method
	/// </summary>
	public static class ConfigurationExtensions
	{
		public static void foreachElements(this IConfigurationRoot configRoot, Action<KeyValuePair<string, string?>> fnIter)
		{
			foreach (KeyValuePair<string, string?> iter in configRoot.AsEnumerable())
			{
				fnIter(iter);
			}
		}

		public static void foreachByPattern(this IConfigurationRoot configRoot, List<CRegexMatchFilterEvent<KeyValuePair<string, string?>>> lstRegexMatchFilterEvent)
		{
			configRoot.foreachElements(
				(iter) =>
				{
					for (int i = 0; i < lstRegexMatchFilterEvent.Count; ++i)
					{
						lstRegexMatchFilterEvent[i].tryOnMatch(iter.Key, iter);
					}
				}
			);
		}

		public static void foreachByPattern(this IConfigurationRoot configRoot, CRegexMatchFilterEvent<KeyValuePair<string, string?>> regexMatchFilterEvent)
		{
			List<CRegexMatchFilterEvent<KeyValuePair<string, string?>>> lstTemp = new();
			lstTemp.Add(regexMatchFilterEvent);

			configRoot.foreachByPattern(lstTemp);
		}

		public static List<string> findKeyByPattern(this IConfigurationRoot configRoot, List<CRegexMatchFilter> lstRegexMatchFilter)
		{
			List<string> lstFindKey = new();

			configRoot.foreachElements(
				(iter) =>
				{
					for (int i = 0; i < lstRegexMatchFilter.Count; ++i)
					{
						if (lstRegexMatchFilter[i].isMatch(iter.Key) == true)
						{
							lstFindKey.Add(iter.Key);
						}
					}
				}
			);

			return lstFindKey;
		}

		public static List<string> findKeyByPattern(this IConfigurationRoot configRoot, CRegexMatchFilter regexMatchFilter)
		{
			List<CRegexMatchFilter> lstTemp = new();
			lstTemp.Add(regexMatchFilter);

			return configRoot.findKeyByPattern(lstTemp);
		}
	}
}
