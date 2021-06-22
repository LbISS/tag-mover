using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace TagMover.Tag.TagProcessors
{
	/// <summary>
	/// Processor for reading Asf tags from files - e.g. .wma
	/// </summary>
	/// <seealso cref="TagMover.Tag.TagProcessors.BaseTagProcessor" />
	/// <seealso cref="TagMover.Tag.ISpecificTagProcessor" />
	public class AsfProcessor : BaseTagProcessor, ISpecificTagProcessor
	{
		protected readonly ILogger<AsfProcessor> _logger;

		public AsfProcessor(
			ILogger<AsfProcessor> logger)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public HashSet<string> SupportedExtensions => new HashSet<string> { ".wma" };

		public override FileTags GetTags(string filePath)
		{
			try
			{
				using (TagLib.File tfile = TagLib.File.Create(filePath))
				{
					var tag = (TagLib.Asf.Tag)tfile.GetTag(TagLib.TagTypes.Asf);
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

		protected FileTags GetAllTags(TagLib.Asf.Tag tag)
		{
			var res = tag
						.GetDescriptors()
						?.ToDictionary(
							k =>
							{
								return k.Name;
							},
							v =>
							{
								return v.ToString();
							}
						);

			return new FileTags(res ?? new Dictionary<string, string>());
		}
	}
}
