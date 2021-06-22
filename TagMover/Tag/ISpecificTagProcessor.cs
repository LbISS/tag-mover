using System.Collections.Generic;

namespace TagMover.Tag
{
	/// <summary>
	/// Reading specific tags from files
	/// </summary>
	/// <seealso cref="TagMover.Tag.ITagProcessor" />
	public interface ISpecificTagProcessor : ITagProcessor
	{
		/// <summary>
		///   <para>
		/// The supported extensions by processor. Processor will try to read tags for files with specified extensions.</para>
		///   <para>Extensions should be specified lowerCase.</para>
		/// </summary>
		HashSet<string> SupportedExtensions { get; }
	}
}
