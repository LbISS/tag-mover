namespace TagMover.Tag
{
	/// <summary>
	/// Reading tags from file
	/// </summary>
	public interface ITagProcessor
	{
		/// <summary>
		/// Reads the tags from the file.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <returns></returns>
		public FileTags GetTags(string filePath);
	}
}
