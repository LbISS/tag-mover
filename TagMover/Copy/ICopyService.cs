using TagMover.Filter;

namespace TagMover.Copy
{
	public interface ICopyService
	{
		void Copy(string sourceFolderPath, string destFolderPath, string includePattern, string excludePattern, FilterFunc filter);
	}
}
