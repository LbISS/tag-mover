using System.IO;

namespace TagMover.Filesystem
{
	public class FilesystemService : IFilesystemService
	{
		public bool isDirectory(string path)
		{
			return Directory.Exists(path);
		}

		public void EnsureDirectory(string path)
		{
			Directory.CreateDirectory(Path.GetDirectoryName(path));
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
