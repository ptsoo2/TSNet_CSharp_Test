using System.Diagnostics;

namespace TSUtil
{
	public class CSystem
	{
		public static double getCpuUsageForProcess(Process process, int delay = 500)
		{
			var startTime = DateTime.UtcNow;
			var startCpuUsage = process.TotalProcessorTime;

			var endTime = DateTime.UtcNow;
			var endCpuUsage = process.TotalProcessorTime;

			var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
			var totalMsPassed = (endTime - startTime).TotalMilliseconds;
			var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);

			return cpuUsageTotal * 100;
		}
	}
}
