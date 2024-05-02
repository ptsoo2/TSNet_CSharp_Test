using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSUtil
{
	public class Latch
	{
		protected readonly object mutex_ = new object();
		protected int count_;

		public Latch(int initializeCount)
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
	}

}
