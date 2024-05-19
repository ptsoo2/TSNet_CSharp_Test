using Serilog;
using Serilog.Context;
using System.Runtime.CompilerServices;

namespace TSUtil
{
	public static class LOG
	{
		public static void VERBOSE(string messageTemplate, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			Log.Logger
				.ForContext("SourceLoc", $"{memberName}({sourceLineNumber.ToString()})")
				.Verbose(messageTemplate);
		}

		public static void DEBUG(string messageTemplate, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			Log.Logger
				.ForContext("SourceLoc", $"{memberName}({sourceLineNumber.ToString()})")
				.Debug(messageTemplate);
		}

		public static void INFO(string messageTemplate, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			Log.Logger
				.ForContext("SourceLoc", $"{memberName}({sourceLineNumber.ToString()})")
				.Information(messageTemplate);
		}

		public static void WARNING(string messageTemplate, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			Log.Logger
				.ForContext("SourceLoc", $"{memberName}({sourceLineNumber.ToString()})")
				.Warning(messageTemplate);
		}

		public static void ERROR(string messageTemplate, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			Log.Logger
				.ForContext("SourceLoc", $"{memberName}({sourceLineNumber.ToString()})")
				.Error(messageTemplate);
		}

		public static void FATAL(string messageTemplate, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			Log.Logger
				.ForContext("SourceLoc", $"{memberName}({sourceLineNumber.ToString()})")
				.Fatal(messageTemplate);
		}
	}

	/// <summary>
	/// 로거에서 function, lineNumber 를 기록하고 싶음
	/// 사용 방법에는 ForContext, PushProperty 두 가지가 있음
	/// 둘 사용 방법에 따른 퍼포먼스 비교
	/// </summary>
	public static class LogPerformance
	{
		// default: 154 ms
		// context1: 229 ms
		// property1: 362 ms
		// context3: 373 ms
		// property3: 747 ms

		public static void Performance_PushProperty1(string messageTemplate, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			using (LogContext.PushProperty("SourceLocation", $"{memberName}({sourceLineNumber.ToString()})"))
			{
				Log.Verbose(messageTemplate);
			}
		}

		public static void Performance_ForContext1_NoFilePath(string messageTemplate, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			Log.Logger
				.ForContext("SourceLocation", $"{memberName}({sourceLineNumber.ToString()})")
				.Verbose(messageTemplate);
		}

		public static void Performance_ForContext1(string messageTemplate, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			Log.Logger
				.ForContext("SourceLocation", $"{memberName}({sourceLineNumber.ToString()})")
				.Verbose(messageTemplate);
		}

		public static void Performance_PushProperty3(string messageTemplate, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			using (LogContext.PushProperty("Func", memberName))
			using (LogContext.PushProperty("File", sourceFilePath))
			using (LogContext.PushProperty("Line", sourceLineNumber.ToString()))
			{
				Log.Verbose(messageTemplate);
			}
		}

		public static void Performance_ForContext3(string messageTemplate, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			Log.Logger
				.ForContext("Func", memberName)
				.ForContext("File", sourceFilePath)
				.ForContext("Line", sourceLineNumber.ToString())
				.Verbose(messageTemplate);
		}
	}
}
