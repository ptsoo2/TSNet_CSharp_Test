﻿using System.Collections.Concurrent;
using System.Diagnostics;

namespace TSUtil
{
	namespace Inheritance.Multiple.Interface
	{
		interface A
		{
			public void a();
		}
		interface B
		{
			public void a();
		}

		class C : A, B
		{
			public void a()
			{ }
		}
	}

	namespace Inheritance.Multiple.Abstract
	{
		abstract class A
		{
			public abstract void a();
		}
		abstract class B
		{
			public abstract void a();
		}

		//class C : A, B	// CS1701: C 클래스는 기본 클래스를 여러개 포함할 수 없습니다.
		//{
		//	public override void a()
		//	{ }
		//}
	}

	namespace Inheritance.Multiple.AbstractInterfaceMix
	{
		interface A
		{
			public abstract void a();
		}
		abstract class B
		{
			public abstract void a();
		}

		class C : B, A
		{
			public override void a()
			{
				// base.a();	// CS0205: 추상 기본 멤버를 호출할 수 없습니다.
			}
		}
	}

	namespace Inheritance.Multiple.AbstractInterfaceMix2
	{
		interface A
		{
			public /*abstract*/ void a();
		}
		abstract class B
		{
			public abstract void a();
		}

		class C : B, A
		{
			public override void a()
			{ }
		}
	}

	namespace Inheritance.Multiple.AbstractInterfaceVirtual
	{
		interface A
		{
			public /*abstract*/ void a();
		}
		abstract class B
		{
			public virtual void a() { }
		}

		class C : B, A
		{
			public override void a()
			{
				base.a();
			}
		}
	}
}

namespace TSUtil
{
	class CParent
	{
		public int a_ = 0;
	}

	class CChild : CParent
	{
	}

	public class Test
	{
		/// <summary>
		/// is 와 == 의 비교(type)
		/// </summary>
		static public void Syntax_IsAndEqualType()
		{
			Console.WriteLine("IsAndEqualType ============");

#pragma warning disable CS0184

			CParent parent = new();
			CChild child = new();
			CParent child2 = new CChild();

			if (parent.GetType() == typeof(CChild)) Console.WriteLine("CChild");
			else Console.WriteLine("not CChild");   //
			if (parent.GetType() == typeof(CParent)) Console.WriteLine("CParent");      //
			else Console.WriteLine("not CParent");
			if (parent.GetType() == typeof(int)) Console.WriteLine("int");
			else Console.WriteLine("not int");      //

			if (child.GetType() == typeof(CChild)) Console.WriteLine("CChild");     //
			else Console.WriteLine("not CChild");
			if (child.GetType() == typeof(CParent)) Console.WriteLine("CParent");
			else Console.WriteLine("not CParent");  // ##
			if (child.GetType() == typeof(int)) Console.WriteLine("int");
			else Console.WriteLine("not int");      //

			if (child2.GetType() == typeof(CChild)) Console.WriteLine("CChild");        // 
			else Console.WriteLine("not CChild");
			if (child2.GetType() == typeof(CParent)) Console.WriteLine("CParent");
			else Console.WriteLine("not CParent");  //  ##
			if (child2.GetType() == typeof(int)) Console.WriteLine("int");
			else Console.WriteLine("not int");      //

			if (parent is CChild) Console.WriteLine("CChild");
			else Console.WriteLine("not CChild");   //
			if (parent is CParent) Console.WriteLine("CParent");        //
			else Console.WriteLine("not CParent");
			if (parent is int) Console.WriteLine("int");
			else Console.WriteLine("not int");      //

			if (child is CChild) Console.WriteLine("CChild");       //
			else Console.WriteLine("not CChild");
			if (child is CParent) Console.WriteLine("CParent");     //  ##
			else Console.WriteLine("not CParent");
			if (child is int) Console.WriteLine("int");
			else Console.WriteLine("not int");      //

			if (child2 is CChild) Console.WriteLine("CChild");      //
			else Console.WriteLine("not CChild");
			if (child2 is CParent) Console.WriteLine("CParent");        //
			else Console.WriteLine("not CParent");
			if (child2 is int) Console.WriteLine("int");
			else Console.WriteLine("not int");      //

#pragma warning restore CS0184
		}

		/// <summary>
		/// is 와 == 의 비교(null)
		/// </summary>
		static public void Syntax_IsAndEqualNull()
		{
			Console.WriteLine("IsAndEqualNull ============");

			CParent a = new();
			CParent? b = null;

			if (a is null) Console.WriteLine("null");
			else Console.WriteLine("Not null");             //

			if (a == null) Console.WriteLine("null");
			else Console.WriteLine("Not null");             //

			if (b is null) Console.WriteLine("null");                   //
			else Console.WriteLine("Not null");

			if (b == null) Console.WriteLine("null");                   //
			else Console.WriteLine("Not null");
		}

		/// <summary>
		/// is 와 == 의 비교(bool)
		/// </summary>
		static public void Syntax_IsAndEqualBool()
		{
			Console.WriteLine("IsAndEqualBool ============");

			bool isA = false;
			bool isB = true;

			if (isA == true) Console.WriteLine("true");
			else Console.WriteLine("false");                //

			if (isB == true) Console.WriteLine("true");             //
			else Console.WriteLine("false");

			if (isA is true) Console.WriteLine("true");
			else Console.WriteLine("false");                //

			if (isB is true) Console.WriteLine("true");             //
			else Console.WriteLine("false");
		}

		/// <summary>
		/// 즉시 실행 람다
		/// </summary>
		static public void Syntax_IIFE()
		{
			int a = 0;
			((Func<int>)(() =>
			{
				a = 23132;
				return a;
			}))();

			Console.WriteLine(a);
		}

		/// <summary>
		/// 람다 내에서 소멸한 인스턴스에 대한 접근
		/// </summary>
		static public void Crash_LambdaScope()
		{
			fnBench_t? bench = null;

			{
				CParent? test = new CParent();
				bench = () => { test.a_ = 123123; };
				test = null;
			}

			// crash!!
			bench();
		}

		/// <summary>
		/// 박싱 성능 테스트
		/// </summary>
		static public void Performance_Boxing()
		{
			const int TEST_ELEMENT_COUNT = 1000000;
			const int TEST_LOOP_COUNT = 5;

			var makeObjList = () =>
			{
				List<object> check = new List<object>();
				for (int seek = 0; seek < TEST_ELEMENT_COUNT; ++seek)
				{
					check.Add(seek);
				}
				return check;
			};

			var makeIntList = () =>
			{
				List<int> check = new List<int>();
				for (int seek = 0; seek < TEST_ELEMENT_COUNT; ++seek)
				{
					check.Add(seek);
				}
				return check;
			};

			Console.WriteLine("`List<object>`");
			Bench(() => { makeObjList(); }, TEST_LOOP_COUNT);

			Console.WriteLine("`List<int>`");
			Bench(() => { makeIntList(); }, TEST_LOOP_COUNT);

			Console.WriteLine("`foreach List<object>`");
			Bench(() =>
				{
					var check = makeObjList();
					foreach (int obj in check)
					{
						var a = obj;
					}
				}, TEST_LOOP_COUNT
			);

			Console.WriteLine("`for List<object>`");
			Bench(() =>
				{
					var check = makeObjList();
					for (int i = 0; i < check.Count; ++i)
					{
						var a = check[i];
					}
				}, TEST_LOOP_COUNT
			);

			Console.WriteLine("`foreach List<int>`");
			Bench(() =>
				{
					var check = makeIntList();
					foreach (int obj in check)
					{
						var a = obj;
					}
				}, TEST_LOOP_COUNT
			);

			Console.WriteLine("`for List<int>`");
			Bench(() =>
				{
					var check = makeIntList();
					for (int i = 0; i < check.Count; ++i)
					{
						var a = check[i];
					}
				}, TEST_LOOP_COUNT
			);
		}

		static public void Performance_Boxing2()
		{
			const int TEST_COUNT = 10000000;

			Bench(
				() =>
				{
					for (int i = 0; i < TEST_COUNT; ++i)
					{
						string test = $"{1123.ToString()}";
					}
				}
			);

			Bench(
				() =>
				{
					for (int i = 0; i < TEST_COUNT; ++i)
					{
						string test = $"{1123}";
					}
				}
			);
		}

		static public void Performance_ConcurrentDictionary()
		{
			const int TEST_COUNT = 5;
			const int THREAD_COUNT = 10;

			for (int i = 0; i < TEST_COUNT; ++i)
			{
				Latch latch = new Latch(THREAD_COUNT);

				ReaderWriterLockSlim lockMap = new();
				Dictionary<int, int> mapTemp = new();

				Bench(
					() =>
					{
						for (int i = 0; i < THREAD_COUNT; ++i)
						{
							Task.Run(
								() =>
								{
									Random random = new();

									int count = 1000000;
									while (count-- > 0)
									{
										if (random.Next() % 2 == 0)
										{
											lock (lockMap)
											{
												mapTemp.TryAdd(random.Next(0, 10), random.Next());
											}
										}
										else
										{
											lock (lockMap)
											{
												int value = 0;
												mapTemp.Remove(random.Next(0, maxValue: 10), out value);
											}
										}
									}
									latch.countdown();
								}
							);
						}

						latch.wait();
					}
				);
			}

			for (int i = 0; i < TEST_COUNT; ++i)
			{
				Latch latch = new Latch(THREAD_COUNT);
				ConcurrentDictionary<int, int> mapTemp = new();

				Bench(
					() =>
					{
						for (int i = 0; i < THREAD_COUNT; ++i)
						{
							Task.Run(
								() =>
								{
									Random random = new();

									int count = 1000000;
									while (count-- > 0)
									{
										if (random.Next() % 2 == 0)
										{
											mapTemp.TryAdd(random.Next(0, 10), random.Next());
										}
										else
										{
											int value = 0;
											mapTemp.Remove(random.Next(0, maxValue: 10), out value);
										}
									}
									latch.countdown();
								}
							);
						}

						latch.wait();
					}
				);
			}
		}

		static public void Performance_IsAndEquals()
		{
			const uint TEST_COUNT = 100000000;

			// is == 속도 테스트(정수)
			{
				int a = 0;
				for (int i = 0; i < 5; ++i)
				{
					Bench(() =>
					{
						for (uint i = 0; i < TEST_COUNT; ++i)
						{
							int test = 0;
							if (a == 0)
							{
								++test;
							}
						}
					});
				}

				for (int i = 0; i < 5; ++i)
				{
					Bench(() =>
					{
						for (uint i = 0; i < TEST_COUNT; ++i)
						{
							int test = 0;
							if (a is 0)
							{
								++test;
							}
						}
					});
				}
			}

			// is == 속도 테스트(str)
			{
				const string str = "31231231321sdfjfolp$^%^^%$";

				Console.WriteLine("==");
				for (int i = 0; i < 5; ++i)
				{
					Bench(() =>
					{
						for (uint i = 0; i < TEST_COUNT; ++i)
						{
							int test = 0;
							if (str == "31231231321sdfjfolp$^%^^%$")
							{
								++test;
							}
						}
					});
				}

				Console.WriteLine("is");
				for (int i = 0; i < 5; ++i)
				{
					Bench(() =>
					{
						for (uint i = 0; i < TEST_COUNT; ++i)
						{
							int test = 0;
#pragma warning disable CS8520
							if (str is "31231231321sdfjfolp$^%^^%$")
							{
								++test;
							}
#pragma warning restore CS8520
						}
					});
				}

				Console.WriteLine("equals");
				for (int i = 0; i < 5; ++i)
				{
					Bench(() =>
					{
						for (uint i = 0; i < TEST_COUNT; ++i)
						{
							int test = 0;
							if (str.Equals("31231231321sdfjfolp$^%^^%$") == true)
							{
								++test;
							}
						}
					});
				}
			}
		}

		/// <summary>
		/// 코드 수행 시간 측정
		/// </summary>		
		public delegate void fnBench_t();
		static public void Bench(fnBench_t fnBench, int count = 1, int intervalMilliseconds = 0)
		{
			if (count < 1)
				throw new ArgumentOutOfRangeException($"Invalid count({count.ToString()})");

			long totalElapsedTime = 0;
			Stopwatch watch = new();

			for (int i = 0; i < count; ++i)
			{
				watch.Restart();
				fnBench();
				watch.Stop();

				long elapsedTime = watch.ElapsedMilliseconds;
				totalElapsedTime += elapsedTime;

				//Console.WriteLine($"[{(i + 1).ToString()}] Elapsed time: {elapsedTime.ToString()} ms");
				Console.WriteLine($"Elapsed time: {elapsedTime.ToString()} ms");

				if (intervalMilliseconds > 0)
					Thread.Sleep(intervalMilliseconds);
			}

			// Console.WriteLine($"Total Elapsed time: {totalElapsedTime.ToString()} ms");
		}

		/// <summary>
		/// 단순 함수 실행 테스트
		/// </summary>
		static public T Eval<T>(Func<T> func)
		{
			return func();
		}

		static void TestAll()
		{
			Syntax_IsAndEqualNull();
			Syntax_IsAndEqualType();
			Syntax_IsAndEqualBool();
			Syntax_IIFE();
			Crash_LambdaScope();
			Performance_IsAndEquals();
			Performance_Boxing();
			Performance_Boxing2();
		}
	}
}
