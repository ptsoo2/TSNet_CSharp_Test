namespace TSUtil
{
	/// <summary>
	/// Serilog 상에서 기본 제공되는 기능에 함수명, 줄 번호를 찍을 수 있는 기능이 없음.
	/// 관련 기능 참고하여 추가
	/// https://github.com/serilog/serilog/wiki/Enrichment
	/// https://stackoverflow.com/questions/77433621/how-to-log-the-classpath-methodname-and-linenumber-with-serilog-in-c-sharp
	/// </summary>
	public static class SourceLocEnricher
	{
		public static string defaultPropertyName = "SourceLoc";
		public static string defaultMessageTemplate = "{0}({1})";

		public static Serilog.ILogger enrich(Serilog.ILogger logger, SourceLocation sourceLocation, string propertyName, string messageTemplate)
		{
			string propertyValue = string.Format(messageTemplate, sourceLocation.func_, sourceLocation.line_.ToString(), sourceLocation.file_);
			return logger.ForContext(propertyName, propertyValue);
		}

		public static Serilog.ILogger enrich(Serilog.ILogger logger, SourceLocation sourceLocation)
		{
			return enrich(logger, sourceLocation, defaultPropertyName, defaultMessageTemplate);
		}
	}
}
