
namespace TSNet
{
	public abstract class COperationBase
	{
		/// <summary>
		/// 시작
		/// </summary>
		/// <returns></returns>
		protected abstract object? _initiate();

		/// <summary>
		/// 완료
		/// </summary>
		/// <param name="result"></param>
		protected abstract void _complete(object? result);

		/// <summary>
		/// 1번 작업 수행
		/// </summary>
		protected void _runOnce()
		{
			object? result = _initiate();
			_complete(result);
		}
	}
}