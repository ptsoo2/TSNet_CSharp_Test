
namespace TSUtil
{
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

	public static class DataUnitExtensions
	{
		// data unit
		public static long BYTE_TO_KILLOBYTE(this long value)
		{
			return DataUnit.BYTE_TO_KILLOBYTE(value);
		}
		public static long KILLOBYTE_TO_MEGABYTE(this long value)
		{
			return DataUnit.KILLOBYTE_TO_MEGABYTE(value);
		}
		public static long MEGABYTE_TO_GIGABYTE(this long value)
		{
			return DataUnit.MEGABYTE_TO_GIGABYTE(value);
		}
		public static long BYTE_TO_MEGABYTE(this long value)
		{
			return DataUnit.BYTE_TO_MEGABYTE(value);
		}
		public static long BYTE_TO_GIGABYTE(this long value)
		{
			return DataUnit.BYTE_TO_GIGABYTE(value);
		}
	}

	public static class TimeUnitExtensions
	{
		// time unit
		public static int SEC_TO_MILLISEC(this int value)
		{
			return TimeUnit.SEC_TO_MILLISEC(value);
		}
		public static int MILLISEC_TO_SEC(this int value)
		{
			return TimeUnit.MILLISEC_TO_SEC(value);
		}
		public static int SEC_TO_MIN(this int value)
		{
			return TimeUnit.SEC_TO_MIN(value);
		}
		public static int MIN_TO_SEC(this int value)
		{
			return TimeUnit.MIN_TO_SEC(value);
		}
	}

	public static class EnumExtensions
	{
		public static int toInt<T>(this T value) where T : IConvertible//enum
		{
			if (typeof(T).IsEnum == false)
			{
				throw new ArgumentException("T must be an enumerated type");
			}

			return (int)(IConvertible)value;
		}
	}
}
