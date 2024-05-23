using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Context;
using Serilog.Core;
using Serilog.Events;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TSUtil
{
	/// <summary>
	/// 함수명, 줄번호, 파일명을 로그 메시지에 추가
	/// </summary>
	public static class SourceLocEnricher
	{
		private static string defaultPropertyName_ = "SourceLoc";
		private static string defaultMessageTemplate_ = "{0}({1})";

		public static ILogger enrich(ILogger logger, SourceLocation sourceLocation, string? propertyName = null, string? messageTemplate = null)
		{
			propertyName ??= defaultPropertyName_;
			messageTemplate ??= defaultMessageTemplate_;

			string propertyValue = string.Format(messageTemplate, sourceLocation.func_, sourceLocation.line_.ToString(), sourceLocation.file_);
			return logger.ForContext(propertyName, propertyValue);
		}
	}

	/// <summary>
	/// SourceLocEnricher 를 사용하는 외부 인터페이스
	/// </summary>
	public static class LoggerExtensions
	{
		public static ILogger attachSourceLocation(this ILogger logger, SourceLocation sourceLocation, string? propertyName = null, string? messageTemplate = null)
		{
			return SourceLocEnricher.enrich(logger, sourceLocation, propertyName, messageTemplate);
		}
	}

	/// <summary>
	/// 로거 전신
	/// </summary>
	/// <typeparam name="TTag"></typeparam>
	public partial class CLoggerBase<TTag>
	{
		protected static Serilog.ILogger logger_ { get; private set; } = null!;

		public static void init(string configPath)
		{
			_throwIfInitialized();

			var config = new ConfigurationBuilder().AddJsonFile(configPath).Build();
			logger_ = new LoggerConfiguration().ReadFrom.Configuration(config).CreateLogger();
		}

		public static void close()
		{
			if (logger_ == null)
				return;

			// ref@ Serilog.Log.CloseAndFlush
			// 내부적으로 Async 절차로 종료됨에 유의
			(logger_ as IDisposable)?.Dispose();
		}

		protected static void _write(LogEventLevel logEventLevel, SourceLocation sourceLocation, string messageTemplate)
		{
			ArgumentNullException.ThrowIfNull(logger_);
			logger_.attachSourceLocation(sourceLocation).Write(logEventLevel, messageTemplate);
		}

		protected static void _throwIfInitialized()
		{
			if (logger_ != null)
				throw new System.Exception("Already initialized");
		}
	}

	/// <summary>
	/// 실제 로깅시 사용하는 부분
	/// 기본 제공되는 기능에 함수명, 줄 번호를 찍을 수 있는 기능이 없음. 관련 기능 참고하여 추가
	/// https://github.com/serilog/serilog/wiki/Enrichment
	/// https://stackoverflow.com/questions/77433621/how-to-log-the-classpath-methodname-and-linenumber-with-serilog-in-c-sharp
	/// </summary>
	/// <typeparam name="TTag"></typeparam>
	public partial class CLoggerBase<TTag>
	{
		/// <summary>
		/// Verbose 레벨 로깅
		/// </summary>
		public static void VERBOSE(string messageTemplate, [CallerMemberName] string func = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
			=> _write(LogEventLevel.Verbose, new SourceLocation(func, file, line), messageTemplate);

		/// <summary>
		/// Debug 레벨 로깅
		/// </summary>
		public static void DEBUG(string messageTemplate, [CallerMemberName] string func = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
			=> _write(LogEventLevel.Debug, new SourceLocation(func, file, line), messageTemplate);

		/// <summary>
		/// Info 레벨 로깅
		/// </summary>
		public static void INFO(string messageTemplate, [CallerMemberName] string func = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
			=> _write(LogEventLevel.Information, new SourceLocation(func, file, line), messageTemplate);

		/// <summary>
		/// Warning 레벨 로깅
		/// </summary>
		public static void WARNING(string messageTemplate, [CallerMemberName] string func = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
			=> _write(LogEventLevel.Warning, new SourceLocation(func, file, line), messageTemplate);

		/// <summary>
		/// Error 레벨 로깅
		/// </summary>
		public static void ERROR(string messageTemplate, [CallerMemberName] string func = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
			=> _write(LogEventLevel.Error, new SourceLocation(func, file, line), messageTemplate);

		/// <summary>
		/// Fatal 레벨 로깅
		/// </summary>
		public static void FATAL(string messageTemplate, [CallerMemberName] string func = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
			=> _write(LogEventLevel.Fatal, new SourceLocation(func, file, line), messageTemplate);
	}

	public struct tagDefaultLogger { }
	public class LOG : CLoggerBase<tagDefaultLogger> { }
}
