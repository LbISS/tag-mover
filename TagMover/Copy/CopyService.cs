﻿using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using TagMover.Filesystem;
using TagMover.Filter;
using TagMover.Tag;

namespace TagMover.Copy
{
	public class CopyService : ICopyService
	{
		protected readonly ILogger<CopyService> _logger;
		protected readonly IFilesystemService _filesystemService;
		protected readonly ITagsService _tagsService;

		public CopyService(IFilesystemService filesystemService,
			ILogger<CopyService> logger,
			ITagsService tagsService)
		{
			_filesystemService = filesystemService ?? throw new ArgumentNullException(nameof(filesystemService));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_tagsService = tagsService ?? throw new ArgumentNullException(nameof(tagsService));
		}

		public void Copy(string sourceFolderPath, string destFolderPath, FilterFunc filter)
		{
			if(!_filesystemService.IsDirectory(sourceFolderPath))
			{
				throw new ArgumentException("The specified source is not a directory");
			}

			if (_filesystemService.IsFileExists(destFolderPath))
			{
				throw new ArgumentException("The specified destination is a file. Please specify a directory.");
			}

			_filesystemService.EnsureDirectory(destFolderPath, false);

			_logger.LogInformation("Starting process...");

			var srcFiles = _filesystemService.GetFiles(sourceFolderPath);
			_logger.LogInformation($"{srcFiles.Length} files found in source folder.");

			var extensions = srcFiles.Select(s => Path.GetExtension(s)).Distinct();

			var filteredFiles = srcFiles.AsParallel().Where(w => filter(w, _tagsService.GetFileTags(w) ?? new FileTags())).ToList();

			_logger.LogInformation($"{filteredFiles.Count} files successfully passed filter and patterns.");
			_logger.LogInformation($"Copying {filteredFiles.Count} files. Please wait, it could take a while...");

			//"Copy" operation limited by disc IO, so there is no point in parallelisation here.
			//Moreover, it could make the results worse for HDDs.
			for (int i = 0; i < filteredFiles.Count; i++)
			{
				string oldPath = filteredFiles[i];
				_logger.LogTrace($"Copying {i}/{filteredFiles.Count} file: '{oldPath}'.");

				var newPath = oldPath.Replace(sourceFolderPath, destFolderPath);
				_filesystemService.EnsureDirectory(newPath, true);
				_filesystemService.CopyFile(oldPath, newPath);

				_logger.LogTrace($"File '{oldPath}' has been copied to the '{newPath}'.");

				if(i != 0 && i % 100 == 0)
				{
					_logger.LogInformation($"Successfully copied {i} files.");
				}
			}

			_logger.LogInformation($"All files successfully copied.");
		}
	}
}
