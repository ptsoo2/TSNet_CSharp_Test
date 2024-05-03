
namespace ThreadType
{
	/// <summary>
	/// ThreadType 으로 value generic 을 사용하기 위함.
	/// Single - 싱글스레드
	/// Multi - 멀티스레드
	/// </summary>
	public interface __ThreadType<T> where T : unmanaged;
	public struct Single : __ThreadType<Single>;
	public struct Multi : __ThreadType<Multi>;
}
