
using System.Diagnostics;
using ThreadType;

namespace TSUtil
{
	/// <summary>
	/// 퍼포먼스 스냅샷 기록값
	/// </summary>
	public struct PerformanceSnapshot
	{
		public ulong id_ = 0;

		// 누적
		public ulong totalCount_ = 0;

		// 단위 시간당
		public ulong nowCount_ = 0;
		public ulong avgCount_ => (id_ < 1) ? 0 : (totalCount_ / id_);
		public ulong estimatedMaxCount_ = 0;

		public long totalElapsedMilliseconds_ = 0;

		public PerformanceSnapshot(ulong snapshotId)
		{
			id_ = snapshotId;
		}
	}

	/// <summary>
	/// 퍼포먼스 측정기
	/// </summary>
	public class CPerformanceMeasurer<T>
		where T : unmanaged, IThreadType<T>
	{
		protected INumberWrapper<ulong> nowCounter_ = CNumberWrapperFactory.create<ulong, T>()!;
		protected INumberWrapper<ulong> totalCounter_ = CNumberWrapperFactory.create<ulong, T>()!;

		/// <summary>
		/// not thread safety
		/// </summary>
		protected INumberWrapper<ulong> estimatedMaxCounter_ = CNumberWrapperFactory.create<ulong, T>()!;

		protected Stopwatch watch_ = new();
		protected long totalElapsedMilliseconds_ = 0;
		protected INumberWrapper<ulong> captureCounter_ = CNumberWrapperFactory.create<ulong, T>()!;

		/// <summary>
		/// thread safety.
		/// now, total 은 별도의 Interlocked 연산이기에 둘 사이의 관계에서 정확함은 보장하지 않음
		/// </summary>
		public ulong nowCount => nowCounter_.value;
		public ulong totalCount => totalCounter_.value;

		/// <summary>
		/// not thread safety
		/// </summary>
		public bool capture(long criteriaElapsedTime, out PerformanceSnapshot outProperties)
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
					outProperties = new PerformanceSnapshot(totalCaptureCount)
					{
						totalCount_ = totalCount,
						nowCount_ = nowCount,
						totalElapsedMilliseconds_ = totalElapsedMilliseconds_
					};

					// max 갱신
					outProperties.estimatedMaxCount_ = estimatedMaxCounter_.value;
					if (outProperties.nowCount_ > outProperties.estimatedMaxCount_)
					{
						// not thread safety
						outProperties.estimatedMaxCount_
							= estimatedMaxCounter_.value
							= outProperties.nowCount_;
					}
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
