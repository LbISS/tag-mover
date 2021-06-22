using System.Collections.Generic;

namespace TagMover.Tag
{
	/// <summary>
	/// Class representing tags and their values
	/// </summary>
	/// <seealso cref="System.Collections.Generic.Dictionary{System.String, System.String}" />
	public class FileTags : Dictionary<string, string>
	{
		public FileTags() : base() { }
		public FileTags(IDictionary<string, string> dictionary) : base(dictionary) { }

		/// <summary>
		/// Merges the specified new tags into existing object.
		/// </summary>
		/// <param name="newTags">The new tags.</param>
		public void Merge(FileTags newTags)
		{
			foreach (var newTag in newTags)
			{
				if(!this.ContainsKey(newTag.Key))
				{
					this[newTag.Key] = newTag.Value;
				}
			}
		}
	}
}
