﻿
namespace TSUtil
{
	/// <summary>
	/// MT 환경에서 counter 기반 wait 용도
	/// </summary>
	public class Latch
	{
		protected readonly object mutex_ = new object();
		protected int count_;

		public Latch(int initializeCount = 1)
		{
			if (initializeCount < 1)
			{
				throw new ArgumentOutOfRangeException("Invalid initialize count");
			}

			count_ = initializeCount;
		}

		public void wait()
		{
			lock (mutex_)
			{
				while (count_ > 0)
				{
					Monitor.Wait(mutex_);
				}
			}
		}

		public void countdown()
		{
			lock (mutex_)
			{
				if (--count_ <= 0)
				{
					Monitor.PulseAll(mutex_);
				}
			}
		}

		public int count()
		{
			lock (mutex_)
			{
				return count_;
			}
		}

		public bool isEnd()
		{
			return count() < 1;
		}
	}

}
