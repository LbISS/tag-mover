namespace TagMover.Filesystem
{
	public interface IFilesystemService
	{
		bool isDirectory(string path);

		void EnsureDirectory(string path);

		void CopyFile(string sourcePath, string targetPath);

		string[] GetFiles(string path, bool getAllRecursively = true);

		string[] GetDirectories(string path, bool getAllRecursively = true);
	}
}
