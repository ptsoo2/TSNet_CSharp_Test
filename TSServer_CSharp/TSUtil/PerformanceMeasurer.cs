
using System.Diagnostics;
using ThreadType;

namespace TSUtil
{
	public struct PerformanceEvent
	{
		public ulong eventId_ = 0;

		public ulong nowCount_ = 0;
		public ulong totalCount_ = 0;
		public ulong avgCount_ => (eventId_ < 1) ? 0 : (totalCount_ / eventId_);

		public long totalElapsedMilliseconds_ = 0;

		public PerformanceEvent(ulong eventId)
		{
			eventId_ = eventId;
		}
	}

	/// <summary>
	/// 퍼포먼스 측정기
	/// </summary>
	public class CPerformanceMeasurer<T>
		where T : unmanaged, IThreadType<T>
	{
		protected ICounter<ulong> nowCounter_ = CounterFactory.create<ulong, T>()!;
		protected ICounter<ulong> totalCounter_ = CounterFactory.create<ulong, T>()!;

		/// <summary>
		/// not thread safety
		/// </summary>
		protected Stopwatch watch_ = new();
		protected long totalElapsedMilliseconds_ = 0;
		protected ICounter<ulong> captureCounter_ = CounterFactory.create<ulong, ThreadType.Single>()!;

		/// <summary>
		/// thread safety.
		/// now, total 은 별도의 Interlocked 연산이기에 둘 사이의 관계에서 정확함은 보장하지 않음
		/// </summary>
		public ulong nowCount => nowCounter_.value;
		public ulong totalCount => totalCounter_.value;

		/// <summary>
		/// not thread safety
		/// </summary>
		public bool capture(long criteriaElapsedTime, out PerformanceEvent outProperties)
		{
			// 실행중이 아니라면 시작
			if (watch_.IsRunning == false)
				watch_.Restart();

			long elapsedMilliseconds = watch_.ElapsedMilliseconds;
			if (elapsedMilliseconds < criteriaElapsedTime)
			{
				outProperties = default;
				return false;
			}

			// 도래했다면, 결과 뽑고,
			{
				totalElapsedMilliseconds_ += elapsedMilliseconds;

				var totalCaptureCount = captureCounter_.increment();
				{
					outProperties = new PerformanceEvent(totalCaptureCount)
					{
						nowCount_ = nowCount,
						totalCount_ = totalCount,
						totalElapsedMilliseconds_ = totalElapsedMilliseconds_
					};
				}
			}

			// 측정 재시작
			resetNowCount();
			watch_.Restart();
			return true;
		}

		/// <summary>
		/// thread safety
		/// </summary>
		public void addCount(ulong value)
		{
			nowCounter_.addCount(value);
			totalCounter_.addCount(value);
		}

		/// <summary>
		/// thread safety
		/// </summary>
		public void incrementCount()
		{
			nowCounter_.increment();
			totalCounter_.increment();
		}

		/// <summary>
		/// thread safety
		/// </summary>
		public void resetNowCount()
		{
			nowCounter_.zeroize();
		}

		/// <summary>
		/// thread safety
		/// </summary>
		public void resetAllCount()
		{
			nowCounter_.zeroize();
			totalCounter_.zeroize();
		}
	}
}

namespace TSUtil
{
	public sealed class CPerformanceMeasurer_ST : CPerformanceMeasurer<ThreadType.Single>;
	public sealed class CPerformanceMeasurer_MT : CPerformanceMeasurer<ThreadType.Multi>;
}
