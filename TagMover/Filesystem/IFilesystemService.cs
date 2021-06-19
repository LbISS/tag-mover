namespace TagMover.Filesystem
{
	public interface IFilesystemService
	{
		bool IsDirectory(string path);

		bool IsFileExists(string path);

		void EnsureDirectory(string path, bool isFilePath);

		void CopyFile(string sourcePath, string targetPath);

		string[] GetFiles(string path, bool getAllRecursively = true);

		string[] GetDirectories(string path, bool getAllRecursively = true);

		/// <summary>Gets the extension of file.</summary>
		/// <param name="path">The file path.</param>
		/// <returns>Extension with period (e.g. ".mp3"). Null or empty string in case of non-existing file, directory or file without extension.</returns>
		string GetExtension(string path);
	}
}
