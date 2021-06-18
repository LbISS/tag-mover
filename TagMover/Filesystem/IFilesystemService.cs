namespace TagMover.Filesystem
{
	public interface IFilesystemService
	{
		bool isDirectory(string path);

		bool isFileExists(string path);

		void EnsureDirectory(string path, bool isFilePath);

		void CopyFile(string sourcePath, string targetPath);

		string[] GetFiles(string path, bool getAllRecursively = true);

		string[] GetDirectories(string path, bool getAllRecursively = true);
	}
}
