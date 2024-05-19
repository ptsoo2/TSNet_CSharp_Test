
namespace TSNet
{
	public abstract class Operation
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

		/// </summary>
		protected virtual void _runOnce()
		/// <summary>
		/// 루틴 실행
		{
			object? result = _initiate();
			_complete(result);
		}
	}
}
