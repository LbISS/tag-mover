using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace TagMover.Tag.TagProcessors
{
	public class XiphProcessor : BaseTagProcessor, ISpecificTagProcessor
	{
		protected readonly ILogger<XiphProcessor> _logger;

		public XiphProcessor(
			ILogger<XiphProcessor> logger)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public HashSet<string> SupportedExtensions => new HashSet<string> { ".ogg" };

		public override FileTags GetTags(string filePath)
		{
			try
			{
				using (TagLib.File tfile = TagLib.File.Create(filePath))
				{
					var tag = (TagLib.Ogg.XiphComment)tfile.GetTag(TagLib.TagTypes.Xiph);
					if (tag == null)
						return null;

					var res = GetAllTags(tag);

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

		protected FileTags GetAllTags(TagLib.Ogg.XiphComment tag)
		{
			var res = tag
						.ToDictionary(
							k => k,
							v => string.Join("; ", tag.GetField(v) ?? Array.Empty<string>())
						);

			return new FileTags(res);
		}
	}
}
