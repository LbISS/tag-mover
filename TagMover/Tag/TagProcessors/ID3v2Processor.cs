using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace TagMover.Tag.TagProcessors
{
	/// <summary>
	/// Processor for reading ID3v2 tags from files - e.g. .mp3
	/// </summary>
	/// <seealso cref="TagMover.Tag.TagProcessors.BaseTagProcessor" />
	/// <seealso cref="TagMover.Tag.ISpecificTagProcessor" />
	public class ID3v2Processor : BaseTagProcessor, ISpecificTagProcessor
	{
		protected readonly ILogger<ID3v2Processor> _logger;

		public ID3v2Processor(
			ILogger<ID3v2Processor> logger)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public HashSet<string> SupportedExtensions => new HashSet<string> { ".mp3", ".wav" };

		public override FileTags GetTags(string filePath)
		{
			try
			{
				using (TagLib.File tfile = TagLib.File.Create(filePath))
				{
					var tag = (TagLib.Id3v2.Tag)tfile.GetTag(TagLib.TagTypes.Id3v2);
					if (tag == null)
						return null;

					var res = GetStandardTags(tag);

					var userTags = GetUserTags(tag);
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

		protected FileTags GetUserTags(TagLib.Id3v2.Tag tag)
		{
			var frames = tag
						.GetFrames(TagLib.ByteVector.FromString("TXXX", TagLib.StringType.UTF8));

			if (frames == null)
				return new FileTags();

			var res = frames
						.Where(w => w is TagLib.Id3v2.UserTextInformationFrame)
						.Select(s => s as TagLib.Id3v2.UserTextInformationFrame)
						.GroupBy(g => g.Description)
						.Select(s2 => s2.First())
						.ToDictionary(
							k => k.Description,
							v => string.Join("; ", v.Text)
						);

			return new FileTags(res);
		}
	}
}
