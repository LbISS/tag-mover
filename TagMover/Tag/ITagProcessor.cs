namespace TagMover.Tag
{
	public interface ITagProcessor
	{
		public FileTags GetTags(string filePath);
	}
}
