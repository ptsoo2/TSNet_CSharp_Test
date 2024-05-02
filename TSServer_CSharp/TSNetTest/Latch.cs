using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSNetTest
{
	public class Latch
	{
		protected readonly object mutex_ = new object();
		protected int count_;

		public Latch(int initializeCount = 1)
		{
			if (initializeCount < 1)
				throw new ArgumentOutOfRangeException("Invalid initialize count");

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

		public void countdown(int downCount = 1)
		{
			if (downCount < 1)
				throw new ArgumentOutOfRangeException($"Invalid countdown count({downCount.ToString()})");

			lock (mutex_)
			{
				count_ -= int.Min(downCount, count_);
				if (count_ < 1)
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
	}

}
