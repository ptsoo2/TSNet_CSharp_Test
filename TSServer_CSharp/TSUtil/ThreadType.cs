namespace ThreadType
{
	/// <summary>
	/// ThreadType 으로 value generic 을 사용하기 위함.
	/// Single - 싱글스레드
	/// Multi - 멀티스레드
	/// </summary>
	public interface IThreadType<T> where T : unmanaged;
	public struct Single : IThreadType<Single>;
	public struct Multi : IThreadType<Multi>;
}
