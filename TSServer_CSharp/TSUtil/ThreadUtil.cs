namespace TSUtil
{
	public class ThreadUtil
	{
		public static void setThreadName(string threadName)
		{
			Thread.CurrentThread.Name = threadName;
		}

		public static void printThreadPoolCountInfo()
		{
			int workerThreads = 0;
			int completionPortThreads = 0;
			ThreadPool.GetMinThreads(out workerThreads, out completionPortThreads);
			Console.WriteLine($"Min Thread count: {workerThreads.ToString()}, {completionPortThreads.ToString()}");

			ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);
			Console.WriteLine($"Max Thread count: {workerThreads.ToString()}, {completionPortThreads.ToString()}");
		}

		//public static void printWithThreadInfo(string? desc = null)
		//{
		//	int threadId = Thread.CurrentThread.ManagedThreadId;
		//	string? threadName = Thread.CurrentThread.Name;

		//	Console.WriteLine($"[{threadId.ToString()}] {desc} `{threadName}`");
		//}
	}
}
