namespace TSNet
{
	/// <summary>
	/// 작업 개념의 기반 클래스
	/// </summary>
	public interface IOperation
	{
		public void run();
	}

	/// <summary>
	/// 비동기 작업 기반 클래스
	/// </summary>
	/// <typeparam name="TResult"></typeparam>
	public interface IAsyncOperation<TResult> : IOperation
	{
		public TResult initiate();
		public void complete(TResult result);
	}
}