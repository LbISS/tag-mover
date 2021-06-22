namespace TagMover.Filesystem
{
	/// <summary>
	/// Abstraction on filesystem to isolate possible specific issues.
	/// </summary>
	public interface IFilesystemService
	{
		/// <summary>
		/// Determines whether the specified path is directory.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns>
		///   <c>true</c> if the specified path is directory; otherwise, <c>false</c>.
		/// </returns>
		bool IsDirectory(string path);

		/// <summary>
		/// Determines whether the file exists on the specified path.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns>
		///   <c>true</c> if file exists on the specified path; otherwise, <c>false</c>.
		/// </returns>
		bool IsFileExists(string path);

		/// <summary>
		/// Creates directory, if no found on the path.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="isFilePath">if set to <c>true</c> then path will be considered as file's path and parent directory for file will be created.</param>
		void EnsureDirectory(string path, bool isFilePath);

		/// <summary>
		/// Copies the file.
		/// </summary>
		/// <param name="sourcePath">The source path.</param>
		/// <param name="targetPath">The target path.</param>
		void CopyFile(string sourcePath, string targetPath);

		/// <summary>
		/// Gets all files on the path.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="getAllRecursively">if set to <c>true</c> then getting all files recursively.</param>
		/// <returns></returns>
		string[] GetFiles(string path, bool getAllRecursively = true);

		/// <summary>
		/// Gets all directories.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="getAllRecursively">if set to <c>true</c> then getting all directories recursively.</param>
		/// <returns></returns>
		string[] GetDirectories(string path, bool getAllRecursively = true);

		/// <summary>Gets the extension of file.</summary>
		/// <param name="path">The file path.</param>
		/// <returns>Extension with period (e.g. ".mp3"). Null or empty string in case of non-existing file, directory or file without extension.</returns>
		string GetExtension(string path);
	}
}
