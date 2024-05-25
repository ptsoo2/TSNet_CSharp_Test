using extensions;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace TSUtil
{
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

			IConfigurationRoot configRoot = new ConfigurationBuilder()
				.AddJsonStream(File.Open(configPath, FileMode.Open))
				.Build();

			// ptsoo todo - 처음 로그 파일 기록시에만 적용되므로 rolling 시에도 적용하는 방법을 알아야 한다.
			_replaceFilePath(configRoot);

			logger_ = new LoggerConfiguration()
				.ReadFrom.Configuration(configRoot)
				.CreateLogger();
		}

		public static void close()
		{
			if (logger_ == null)
				return;

			// ref@ Serilog.Log.CloseAndFlush
			// 내부적으로 Async 절차로 종료됨에 유의
			(logger_ as IDisposable)?.Dispose();
		}

		protected static void _replaceFilePath(IConfigurationRoot root)
		{
			root.foreachByPattern("^Serilog:WriteTo.*Args:path$", RegexOptions.IgnoreCase,
				(key, value) =>
				{
					root[key] = value?.Replace("{timestamp}", DateTime.Now.ToString("yyyyMMdd_HHmmss"));
				}
			);
		}

		protected static void _write(LogEventLevel logEventLevel, SourceLocation sourceLocation, string messageTemplate)
		{
			ArgumentNullException.ThrowIfNull(logger_);
			SourceLocEnricher.enrich(logger_, sourceLocation).Write(logEventLevel, messageTemplate);
		}

		protected static void _throwIfInitialized()
		{
			if (logger_ != null)
				throw new System.Exception("Already initialized");
		}
	}

	/// <summary>
	/// 실제 로깅시 사용하는 부분
	/// </summary>
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
}

namespace TSUtil
{
	/// <summary>
	/// 기본 Logger class
	/// </summary>
	public struct tagDefaultLogger { }
	public class LOG : CLoggerBase<tagDefaultLogger> { }
}
