namespace TSNet
{
	/// <summary>
	/// 세션 컨피그
	/// </summary>
	public class CSessionConfig
	{
		/// <summary>
		/// 소켓 설정
		/// </summary>
		public CSocketOptionConfig socketOption_;

		/// <summary>
		/// 세션 단 수신 버퍼 크기(Application Layer)
		/// </summary>
		public int receiveBufferSize_ = 1 << 14;
	}
}
