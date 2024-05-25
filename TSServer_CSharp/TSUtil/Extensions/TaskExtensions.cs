namespace TSUtil
{
	/// <summary>
	/// Task 관련 Extension method
	/// </summary>
	public static class TaskExtensions
	{
		public static void DoNothing(this Task task)
		{ }

		public static void DoNothing(this ValueTask task)
		{ }

		public static void DetectThrowOnDispose(this Task task)
		{
			// https://forum.dotnetdev.kr/t/task-exception/5345/3
			// 혹시나 감지 못한 throw 에 의해서 Task 가 종료되는 경우
			// 최종적으로 발생한 Exception 이 있는지 확인한다.
			task.ContinueWith(
				(innerTask) =>
				{
					AggregateException? exception = innerTask.Exception;
					if (exception != null)
					{
						LOG.ERROR(exception.ToString());
					}
				}
			);
		}
	}
}