namespace TagMover.Tag
{
	/// <summary>
	/// Service responsible for gettings metadata tags from files
	/// </summary>
	public interface ITagsService
	{
		/// <summary>
		/// Gets the file tags for the file.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <returns></returns>
		public FileTags GetFileTags(string filePath);
	}
}
