﻿namespace TSNet
{
	public class CTCPSession : IDisposable
	{
		private bool isDisposed_ = false;

		// // TODO: 비관리형 리소스를 해제하는 코드가 'Dispose(bool disposing)'에 포함된 경우에만 종료자를 재정의합니다.
		~CTCPSession()
		{
			// 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
			Dispose(isManualDispose: false);
		}

		protected virtual void Dispose(bool isManualDispose)
		{
			if (isDisposed_ == true)
				return;

			if (isManualDispose == true)
			{
				// TODO: 관리형 상태(관리형 개체)를 삭제합니다.
			}

			// TODO: 비관리형 리소스(비관리형 개체)를 해제하고 종료자를 재정의합니다.
			// TODO: 큰 필드를 null로 설정합니다.
			isDisposed_ = true;
		}

		public void Dispose()
		{
			// 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
			Dispose(isManualDispose: true);
			GC.SuppressFinalize(this);
		}
	}
}
