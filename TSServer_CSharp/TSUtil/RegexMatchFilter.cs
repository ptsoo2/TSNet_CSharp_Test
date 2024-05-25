using System.Text.RegularExpressions;

namespace TSUtil
{
	public class CRegexMatchFilter
	{
		public readonly string pattern_;
		public readonly RegexOptions regexOption_;

		public CRegexMatchFilter(string pattern, RegexOptions regexOption)
		{
			pattern_ = pattern;
			regexOption_ = regexOption;
		}

		public bool isMatch(string key)
		{
			return Regex.IsMatch(key, pattern_, regexOption_);
		}
	}

	public class CRegexMatchFilterEvent<TData> : CRegexMatchFilter
	{
		public readonly Action<TData> fnOnMatch_;

		public CRegexMatchFilterEvent(string pattern, RegexOptions regexOption, Action<TData> fnOnMatch)
			: base(pattern, regexOption)
		{
			fnOnMatch_ = fnOnMatch;
		}

		public bool tryOnMatch(string key, TData data)
		{
			if (isMatch(key) == false)
				return false;

			fnOnMatch_(data);
			return true;
		}
	}
}
