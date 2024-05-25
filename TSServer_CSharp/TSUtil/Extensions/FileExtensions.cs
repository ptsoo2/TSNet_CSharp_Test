namespace TSUtil
{
	public static class FileExtensions
	{
		public static void moveFiles(string sourcePath, string destPath, bool isCreateDestPath = false, CRegexMatchFilter? regexMatchFilter = null)
		{
			// src 경로가 존재하지 않으면 옮길 이유가 없다.
			if (Directory.Exists(sourcePath) == false)
				return;

			if (Directory.Exists(destPath) == false)
			{
				// dst 경로가 없는데 새로 생성하지 않는다면 throw
				if (isCreateDestPath == false)
					throw new ArgumentException($"Not exist dest directory(path: {destPath})");

				Directory.CreateDirectory(destPath);
			}

			string[] lstFilePath = Directory.GetFiles(sourcePath);
			for (int i = 0; i < lstFilePath.Length; ++i)
			{
				string filePath = lstFilePath[i];
				if (regexMatchFilter != null)
				{
					if (regexMatchFilter.isMatch(filePath) == false)
						continue;
				}

				string fileName = Path.GetFileName(filePath);
				File.Move(filePath, $"{destPath}/{fileName}");
			}
		}
	}
}
