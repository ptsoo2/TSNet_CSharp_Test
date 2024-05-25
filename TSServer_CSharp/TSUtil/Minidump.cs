using System.Runtime.InteropServices;

namespace TSUtil
{
	/// <summary>
	/// 윈도 환경에서 비정상 종료시 덤프 기록
	/// ref@ https://kukuta.tistory.com/412
	/// </summary>
	public class Minidump
	{
		[Flags]
		public enum _MINIDUMP_TYPE
		{
			MiniDumpNormal = 0x00000000,
			MiniDumpWithDataSegs = 0x00000001,
			MiniDumpWithFullMemory = 0x00000002,
			MiniDumpWithHandleData = 0x00000004,
			MiniDumpFilterMemory = 0x00000008,
			MiniDumpScanMemory = 0x00000010,
			MiniDumpWithUnloadedModules = 0x00000020,
			MiniDumpWithIndirectlyReferencedMemory = 0x00000040,
			MiniDumpFilterModulePaths = 0x00000080,
			MiniDumpWithProcessThreadData = 0x00000100,
			MiniDumpWithPrivateReadWriteMemory = 0x00000200,
			MiniDumpWithoutOptionalData = 0x00000400,
			MiniDumpWithFullMemoryInfo = 0x00000800,
			MiniDumpWithThreadInfo = 0x00001000,
			MiniDumpWithCodeSegs = 0x00002000,
			MiniDumpWithoutAuxiliaryState = 0x00004000,
			MiniDumpWithFullAuxiliaryState = 0x00008000,
			MiniDumpWithPrivateWriteCopyMemory = 0x00010000,
			MiniDumpIgnoreInaccessibleMemory = 0x00020000,
			MiniDumpWithTokenInformation = 0x00040000,
			MiniDumpValidTypeFlags = 0x0007ffff,
		};

		// Pack=4 is important! So it works also for x64!
		[StructLayout(LayoutKind.Sequential, Pack = 4)]
		struct MiniDumpExceptionInformation
		{
			public uint ThreadId;
			public IntPtr ExceptionPointers;
			[MarshalAs(UnmanagedType.Bool)]
			public bool ClientPointers;
		}

		[DllImport("dbghelp.dll",
		  EntryPoint = "MiniDumpWriteDump",
		  CallingConvention = CallingConvention.StdCall,
		  CharSet = CharSet.Unicode,
		  ExactSpelling = true,
		  SetLastError = true)]
		static extern bool MiniDumpWriteDump(
		  IntPtr hProcess,
		  uint processId,
		  IntPtr hFile,
		  _MINIDUMP_TYPE dumpType,
		  ref MiniDumpExceptionInformation expParam,
		  IntPtr userStreamParam,
		  IntPtr callbackParam);

		[DllImport("kernel32.dll", EntryPoint = "GetCurrentThreadId", ExactSpelling = true)]
		static extern uint GetCurrentThreadId();

		[DllImport("kernel32.dll", EntryPoint = "GetCurrentProcess", ExactSpelling = true)]
		static extern IntPtr GetCurrentProcess();

		[DllImport("kernel32.dll", EntryPoint = "GetCurrentProcessId", ExactSpelling = true)]
		static extern uint GetCurrentProcessId();

		[DllImport("kernel32.dll", EntryPoint = "TerminateProcess", ExactSpelling = true)]
		static extern Int32 TerminateProcess(IntPtr hprocess, Int32 ExitCode);

		public static void install_self_mini_dump(bool isTerminate = true)
		{
			MiniDumpExceptionInformation exp;

			exp.ThreadId = GetCurrentThreadId();
			exp.ClientPointers = false;
			exp.ExceptionPointers = Marshal.GetExceptionPointers();

			//덤프 파일 이름
			var dt = DateTime.Now.ToString("yyyy.MM.dd.hh.mm.ss");
			var filePath = $"../dump/{dt}.dmp";

			{
				string? directoryPath = Path.GetDirectoryName(filePath);
				if (directoryPath != null && Directory.Exists(directoryPath) == false)
					Directory.CreateDirectory(directoryPath);
			}

			//덤프 파일 만들기
			using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				bool bRet = MiniDumpWriteDump(
				  GetCurrentProcess(),
				  GetCurrentProcessId(),
				  fs.SafeFileHandle.DangerousGetHandle(),
				  //덤프 파일에 포함할 정보 설정
				  (
					_MINIDUMP_TYPE.MiniDumpWithDataSegs |
					_MINIDUMP_TYPE.MiniDumpWithFullMemory |
					_MINIDUMP_TYPE.MiniDumpWithHandleData |
					_MINIDUMP_TYPE.MiniDumpScanMemory |
					_MINIDUMP_TYPE.MiniDumpWithUnloadedModules |
					_MINIDUMP_TYPE.MiniDumpWithIndirectlyReferencedMemory |
					_MINIDUMP_TYPE.MiniDumpWithProcessThreadData |
					_MINIDUMP_TYPE.MiniDumpWithPrivateReadWriteMemory |
					_MINIDUMP_TYPE.MiniDumpWithFullMemoryInfo |
					_MINIDUMP_TYPE.MiniDumpWithThreadInfo |
					_MINIDUMP_TYPE.MiniDumpWithCodeSegs |
					_MINIDUMP_TYPE.MiniDumpWithFullAuxiliaryState |
					_MINIDUMP_TYPE.MiniDumpWithPrivateWriteCopyMemory |
					_MINIDUMP_TYPE.MiniDumpWithTokenInformation
				  ),
				  ref exp,
				  IntPtr.Zero,
				  IntPtr.Zero);
			}

			//프로그램 강제 종료
			if (isTerminate == true)
			{
				TerminateProcess(GetCurrentProcess(), 0);
			}
		}

		public static void init()
		{
			AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs e) =>
			{
				install_self_mini_dump();
			};
		}
	}
}
