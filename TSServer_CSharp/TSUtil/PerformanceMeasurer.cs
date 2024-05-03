
using System.Diagnostics;

namespace TSUtil
{
	public struct PerformanceProperties
	{
		public ulong nowCount_ = 0;
		public ulong totalCount_ = 0;
		public long totalElapsedMilliseconds_ = 0;

		public PerformanceProperties()
		{ }
	}

	/// <summary>
	/// 퍼포먼스 측정기
	/// </summary>
	public class CPerformanceMeasurer<T>
		where T : unmanaged, ThreadType.__ThreadType<T>
	{
		protected ICounter<ulong>? nowCounter_ = CounterFactory.create<ulong, T>();
		protected ICounter<ulong>? totalCounter_ = CounterFactory.create<ulong, T>();

		/// <summary>
		/// not thread safety
		/// </summary>
		protected Stopwatch watch_ = new();
		protected long totalElapsedMilliseconds_ = 0;

		/// <summary>
		/// thread safety.
		/// now, total 은 별도의 Interlocked 연산이기에 둘 사이의 관계에서 정확함은 보장하지 않음
		/// </summary>
		public ulong nowCount => (nowCounter_ is null) ? 0 : nowCounter_.value;
		public ulong totalCount => (totalCounter_ is null) ? 0 : totalCounter_.value;
		public PerformanceProperties properties
		{
			get
			{
				PerformanceProperties properties;
				properties.nowCount_ = nowCount;
				properties.totalCount_ = totalCount;
				properties.totalElapsedMilliseconds_ = totalElapsedMilliseconds_;
				return properties;
			}
		}

		/// <summary>
		/// not thread safety
		/// </summary>
		public bool update(long criteriaElapsedTime, out PerformanceProperties outProperties)
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
				outProperties = properties;
			}

			// 측정 재시작
			resetNowCount();
			watch_.Restart();
			return true;
		}

		/// <summary>
		/// thread safety
		/// </summary>
		public void incrementCount()
		{
			nowCounter_?.increment();
			totalCounter_?.increment();
		}

		/// <summary>
		/// thread safety
		/// </summary>
		public void resetNowCount()
		{
			nowCounter_?.zeroize();
		}

		/// <summary>
		/// thread safety
		/// </summary>
		public void resetAllCount()
		{
			nowCounter_?.zeroize();
			totalCounter_?.zeroize();
		}
	}
}
