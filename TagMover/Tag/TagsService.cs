using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using TagMover.Filesystem;

namespace TagMover.Tag
{
	public enum MediaTypes
	{
		Audio,
		Video,
		Image
	}


	public class TagsService : ITagsService
	{
		protected readonly ILogger<TagsService> _logger;
		protected readonly IFilesystemService _filesystemService;
		protected readonly IEnumerable<ITagProcessor> _tagsProcessors;

		public TagsService(IFilesystemService filesystemService,
			ILogger<TagsService> logger,
			IEnumerable<ITagProcessor> tagsProcessors)
		{
			_filesystemService = filesystemService ?? throw new ArgumentNullException(nameof(filesystemService));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_tagsProcessors = tagsProcessors ?? throw new ArgumentNullException(nameof(tagsProcessors));
		}

		public FileTags GetFileTags(string filePath)
		{
			var extension = _filesystemService.GetExtension(filePath);
			if(extension == null)
			{
				_logger.LogInformation($"{filePath} is directory or doesn't have extension.");
				return null;
			}
			var processors = this.GetTagProcessorsForExtension(extension);
			if (processors == null || processors.Count == 0)
			{
				_logger.LogInformation($"{filePath} doesn't have suitable processors.");
				return null;
			}

			/* TODO: Support all from https://github.com/mono/taglib-sharp/blob/main/README.md 
Video: mkv, ogv, avi, wmv, asf, mp4(m4p, m4v), mpeg(mpg, mpe, mpv, mpg, m2v)
Audio: aa, aax, aac, aiff, ape, dsf, flac, m4a, m4b, m4p, mp3, mpc, mpp, ogg, oga, wav, wma, wv, webm
Images: bmp, gif, jpeg, pbm, pgm, ppm, pnm, pcx, png, tiff, dng, svg*/
			var tags = new FileTags();
			foreach (var processor in processors)
			{
				//TODO: Possible optimization - share tFile (taglib) instance between processors
				//TODO: Possible optimization - get only tags which used in filter
				var newTags = processor.GetTags(filePath);
				if(newTags != null)
				{
					tags.Merge(newTags);
				}
			}
			return tags;
		}

		protected List<ITagProcessor> GetTagProcessorsForExtension(string extension)
		{
			return _tagsProcessors.Where(w => w.SupportedExtensions.Contains(extension.ToLower())).ToList();
		}
	}
}
