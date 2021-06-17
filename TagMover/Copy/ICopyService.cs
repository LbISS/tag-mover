using TagMover.Filter;

namespace TagMover.Copy
{
	public interface ICopyService
	{
		void Copy(string sourceFolderPath, string destFolderPath, FilterFunc filter);
	}
}
