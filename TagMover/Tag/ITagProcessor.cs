using System.Collections.Generic;

namespace TagMover.Tag
{
	public interface ITagProcessor
	{
		/// <summary>
		///   <para>
		/// The supported extensions by processor. Processor will try to read tags for files with specified extensions.</para>
		///   <para>Extensions should be specified lowerCase.</para>
		/// </summary>
		HashSet<string> SupportedExtensions { get; }
		public FileTags GetTags(string filePath);
	}
}
