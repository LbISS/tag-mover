using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TagMover.Tag.TagProcessors
{
	/// <summary>
	/// Base processor allowing to read tags from any file not covered by any other specific processor
	/// </summary>
	/// <seealso cref="TagMover.Tag.ITagProcessor" />
	public class BaseTagProcessor : ITagProcessor
	{
		public virtual FileTags GetTags(string filePath)
		{
			try
			{
				using (TagLib.File tfile = TagLib.File.Create(filePath))
				{
					var tag = tfile.Tag;
					if (tag == null)
						return null;

					return this.GetStandardTags(tag);
				}
			}
			catch (TagLib.UnsupportedFormatException)
			{
				// as it's backup option - generally we don't care
				return null;
			}
			catch (TagLib.CorruptFileException)
			{
				// as it's backup option - generally we don't care
				return null;
			}
			catch (Exception)
			{
				// as it's backup option - generally we don't care
				return null;
			}
		}

		protected FileTags GetStandardTags(TagLib.Tag tag)
		{
			var virtualPublicProps = tag
										.GetType()
										.GetProperties(BindingFlags.Public | BindingFlags.Instance)
										.Where(w => w.GetMethod.IsPublic && w.GetMethod.IsVirtual);

			var res = new FileTags();

			foreach (var prop in virtualPublicProps)
			{
				var value = prop.GetValue(tag);
				var isCollection = IsPropertyACollection(prop);
				if (value != null && (!isCollection || ((IEnumerable<object>)value).Any()))
				{
					res.Add(
						prop.Name,
						IsPropertyACollection(prop)
							? string.Join("; ", ((IEnumerable<object>)value).Select(s => s?.ToString()))
							: value.ToString()
					);
				}
			}

			return res;
		}

		protected bool IsPropertyACollection(PropertyInfo property)
		{
			return !typeof(string).Equals(property.PropertyType) &&
				typeof(IEnumerable).IsAssignableFrom(property.PropertyType);
		}
	}
}
