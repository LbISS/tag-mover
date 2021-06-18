using System.IO;

namespace TagMover.Filesystem
{
	public class FilesystemService : IFilesystemService
	{
		public bool isDirectory(string path)
		{
			return Directory.Exists(path) && !File.Exists(path);
		}

		public bool isFileExists(string path)
		{
			return File.Exists(path);
		}

		public void EnsureDirectory(string path, bool isFilePath)
		{
			if (Directory.Exists(path))
				return;

			if(isFilePath)
				Directory.CreateDirectory(Path.GetDirectoryName(path));
			else 
				Directory.CreateDirectory(path);
		}

		public void CopyFile(string sourcePath, string targetPath)
		{
			File.Copy(sourcePath, targetPath, true);
		}

		public string[] GetFiles(string path, bool getAllRecursively = true)
		{
			return Directory.GetFiles(path, "*", getAllRecursively ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
		}

		public string[] GetDirectories(string path, bool getAllRecursively = true)
		{
			return Directory.GetDirectories(path, "*", getAllRecursively ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
		}
	}
}
