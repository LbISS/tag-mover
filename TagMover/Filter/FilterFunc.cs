using TagMover.Tag;

namespace TagMover.Filter
{
	/// <summary>
	/// Function used to filter files
	/// </summary>
	/// <param name="filePath">The file path.</param>
	/// <param name="tags">The tags for the file.</param>
	/// <returns></returns>
	public delegate bool FilterFunc(string filePath, FileTags tags);
}
