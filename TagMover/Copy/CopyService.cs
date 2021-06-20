using System;
using System.Linq;
using System.Text.RegularExpressions;
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

		public void Copy(string sourceFolderPath, string destFolderPath,string includePattern, string excludePattern, FilterFunc filter)
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

			Regex includeRegexp = !String.IsNullOrEmpty(includePattern) ? new Regex(includePattern, RegexOptions.Compiled) : null;
			Regex excludeRegexp = !String.IsNullOrEmpty(excludePattern) ? new Regex(excludePattern, RegexOptions.Compiled) : null;

			var srcFiles = _filesystemService.GetFiles(sourceFolderPath);
			_logger.LogInformation($"{srcFiles.Length} files found in source folder.");

			var filteredFiles = srcFiles.Where(w => !ExcludedByPatterns(includeRegexp, excludeRegexp, w)).ToList();

			_logger.LogInformation($"{filteredFiles.Count} files successfully passed patterns.");

			filteredFiles = filteredFiles.Where(w => filter(w, _tagsService.GetFileTags(w) ?? new FileTags())).ToList();

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

		protected bool ExcludedByPatterns(Regex includeRegexp, Regex excludeRegexp, string filePath)
		{
			if (excludeRegexp != null && excludeRegexp.IsMatch(filePath))
			{
				_logger.LogDebug($"'{filePath}': file will be skipped, passing exclude pattern.");
				return true;
			}

			if (includeRegexp != null && !includeRegexp.IsMatch(filePath))
			{
				_logger.LogDebug($"'{filePath}': file will be skipped, not passing include pattern.");
				return true;
			}

			return false;
		}
	}
}
