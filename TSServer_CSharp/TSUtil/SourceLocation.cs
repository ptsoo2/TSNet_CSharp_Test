using System.Runtime.CompilerServices;

namespace TSUtil
{
	public class SourceLocation
	{
		public string func_ { get; private set; }
		public string file_ { get; private set; }
		public int line_ { get; private set; }

		public SourceLocation(string func, string file, int line)
		{
			func_ = func;
			file_ = file;
			line_ = line;
		}

		public static SourceLocation current([CallerMemberName] string func = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
		{
			return new SourceLocation(func, file, line);
		}
	}
}
