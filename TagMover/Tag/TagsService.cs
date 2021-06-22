using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using TagMover.Filesystem;
using TagMover.Tag.TagProcessors;

namespace TagMover.Tag
{
	/// <summary>
	/// Service responsible for gettings metadata tags from files
	/// </summary>
	/// <seealso cref="TagMover.Tag.ITagsService" />
	public class TagsService : ITagsService
	{
		protected readonly ILogger<TagsService> _logger;
		protected readonly IFilesystemService _filesystemService;
		protected readonly IEnumerable<ISpecificTagProcessor> _tagsProcessors;
		protected readonly BaseTagProcessor _baseTagProcessor;

		public TagsService(IFilesystemService filesystemService,
			ILogger<TagsService> logger,
			IEnumerable<ISpecificTagProcessor> tagsProcessors,
			BaseTagProcessor baseTagProcessor)
		{
			_filesystemService = filesystemService ?? throw new ArgumentNullException(nameof(filesystemService));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_tagsProcessors = tagsProcessors ?? throw new ArgumentNullException(nameof(tagsProcessors));
			_baseTagProcessor = baseTagProcessor ?? throw new ArgumentNullException(nameof(baseTagProcessor));
		}

		/// <summary>
		/// Gets the file tags for the file.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <returns></returns>
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
			var processors = _tagsProcessors.Where(w => w.SupportedExtensions.Contains(extension.ToLower())).Select(s => s as ITagProcessor).ToList();
			return processors.Count != 0 ? processors : new List<ITagProcessor> { this._baseTagProcessor };
		}
	}
}
