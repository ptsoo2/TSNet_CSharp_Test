using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSUtil;

namespace TSNet
{
 	public abstract class CIOHandlerBase
	{
		/// <summary>
		/// 시작
		/// </summary>
		/// <returns></returns>
		protected abstract object?	_generateIO();

		/// <summary>
		/// 완료
		/// </summary>
		/// <param name="result"></param>
		protected abstract void		_onCompletionIO(object? result);

		/// <summary>
		/// 루틴 실행
		/// </summary>
		protected virtual void		_processIO()
		{
			object? result = _generateIO();
			_onCompletionIO(result);
		}
	}
}
