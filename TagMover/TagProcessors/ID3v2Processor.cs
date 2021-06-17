using System;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace TagMover.TagProcessors
{
	public class ID3v2Processor
	{
		protected readonly ILogger<ID3v2Processor> _logger;

		public ID3v2Processor(
			ILogger<ID3v2Processor> logger)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
			catch (TagLib.UnsupportedFormatException ex)
			{
				_logger.LogInformation($"'{filePath}': UnsupportedFormatException.");
				return null;
			}
			catch (TagLib.CorruptFileException ex)
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
