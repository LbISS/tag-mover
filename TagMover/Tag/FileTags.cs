using System.Collections.Generic;

namespace TagMover.Tag
{
	public class FileTags : Dictionary<string, string>
	{
		public FileTags() : base() { }
		public FileTags(IDictionary<string, string> dictionary) : base(dictionary) { }

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
