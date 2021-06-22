using TagMover.Filter;

namespace TagMover.Copy
{
	/// <summary>
	/// Service responsible for copy logic
	/// </summary>
	public interface ICopyService
	{
		/// <summary>
		/// Copies the specified source folder to the specified destionation path.
		/// </summary>
		/// <param name="sourceFolderPath">The source folder path.</param>
		/// <param name="destFolderPath">The destination folder path.</param>
		/// <param name="includePattern">The include pattern.</param>
		/// <param name="excludePattern">The exclude pattern.</param>
		/// <param name="filter">The filter.</param>
		void Copy(string sourceFolderPath, string destFolderPath, string includePattern, string excludePattern, FilterFunc filter);
	}
}
