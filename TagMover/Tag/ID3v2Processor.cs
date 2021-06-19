using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace TagMover.Tag
{
	public class ID3v2Processor : ITagProcessor
	{
		protected readonly ILogger<ID3v2Processor> _logger;

		public ID3v2Processor(
			ILogger<ID3v2Processor> logger)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public HashSet<string> SupportedExtensions => new HashSet<string> { ".mp3" };

		public FileTags GetTags(string filePath)
		{
			try
			{
				using (TagLib.File tfile = TagLib.File.Create(filePath))
				{
					var id3v2Tag = (TagLib.Id3v2.Tag)tfile.GetTag(TagLib.TagTypes.Id3v2);
					if(id3v2Tag == null)
						return null;

					var res = GetStandardTags(id3v2Tag);

					var userTags = GetUserTags(id3v2Tag);
					res.Merge(userTags);

					return res;
				}
			}
			catch (TagLib.UnsupportedFormatException)
			{
				_logger.LogInformation($"'{filePath}': UnsupportedFormatException.");
				return null;
			}
			catch (TagLib.CorruptFileException)
			{
				_logger.LogInformation($"'{filePath}': CorruptFileException.");
				return null;
			}
			catch (Exception ex)
			{
				_logger.LogInformation($"'{filePath}': other exception: {ex}.");
				return null;
			}
		}

		protected FileTags GetStandardTags(TagLib.Id3v2.Tag id3v2Tag)
		{
			var virtualPublicProps = id3v2Tag
										.GetType()
										.GetProperties(BindingFlags.Public | BindingFlags.Instance)
										.Where(w => w.GetMethod.IsPublic && w.GetMethod.IsVirtual);

			var res = new FileTags();

			foreach (var prop in virtualPublicProps)
			{
				var value = prop.GetValue(id3v2Tag);
				if(value != null)
				{
					res.Add(
						prop.Name, 
						this.IsPropertyACollection(prop) 
							? String.Join("; ", ((IEnumerable<object>)value).Select(s => s?.ToString())) 
							: value.ToString()
					);
				}
			}

			return res;
		}

		protected bool IsPropertyACollection(PropertyInfo property)
		{
			return (!typeof(String).Equals(property.PropertyType) &&
				typeof(IEnumerable).IsAssignableFrom(property.PropertyType));
		}

		protected FileTags GetUserTags(TagLib.Id3v2.Tag id3v2Tag)
		{
			var res = id3v2Tag
						?.GetFrames(TagLib.ByteVector.FromString("TXXX", TagLib.StringType.UTF8))
						?.ToDictionary(
							k => {
								var kut = k as TagLib.Id3v2.UserTextInformationFrame;
								return kut.Description;
							},
							v => {
								var vut = v as TagLib.Id3v2.UserTextInformationFrame;
								return String.Join("; ", vut.Text);
							}
						);

			return new FileTags(res);
		}

		public TagLib.Id3v2.UserTextInformationFrame GetUserTag(string filePath, string fieldName)
		{
			try
			{
				using (var tfile = TagLib.File.Create(filePath))
				{
					var id3v2_tag = (TagLib.Id3v2.Tag)tfile.GetTag(TagLib.TagTypes.Id3v2);

					var fieldTag = (TagLib.Id3v2.UserTextInformationFrame)id3v2_tag
								?.GetFrames(TagLib.ByteVector.FromString("TXXX", TagLib.StringType.UTF8))
								?.FirstOrDefault(f => {
									return f is TagLib.Id3v2.UserTextInformationFrame utFrame
											&& utFrame.Description.Equals(fieldName, StringComparison.OrdinalIgnoreCase);
								});

					return fieldTag;
				}
			}
			catch (TagLib.UnsupportedFormatException)
			{
				_logger.LogInformation($"'{filePath}': UnsupportedFormatException.");
				return null;
			}
			catch (TagLib.CorruptFileException)
			{
				_logger.LogInformation($"'{filePath}': CorruptFileException.");
				return null;
			}
			catch (Exception ex)
			{
				_logger.LogInformation($"'{filePath}': other exception: {ex}.");
				return null;
			}
		}
	}
}
