using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using TagMover.Filesystem;
using TagMover.Filter;

namespace TagMover.Copy
{
	public class CopyService : ICopyService
	{
		protected readonly ILogger<CopyService> _logger;
		protected readonly IFilesystemService _filesystemService;

		public CopyService(IFilesystemService filesystemService,
			ILogger<CopyService> logger)
		{
			_filesystemService = filesystemService ?? throw new ArgumentNullException(nameof(filesystemService));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public void Copy(string sourceFolderPath, string destFolderPath, FilterFunc filter)
		{
			if(!_filesystemService.isDirectory(sourceFolderPath))
			{
				throw new ArgumentException("The specified source is nto a directory");
			}

			if (!_filesystemService.isDirectory(destFolderPath))
			{
				throw new ArgumentException("The specified destination is nto a directory");
			}

			_logger.LogInformation("Starting process...");
			//Copy all directories
			/*foreach (string dirPath in GetDirectories(sourcePath))
			{
				EnsureDirectory(dirPath.Replace(sourcePath, targetPath));
			}*/

			//Copy all the files & Replaces any files with the same name

			var srcFiles = _filesystemService.GetFiles(sourceFolderPath);
			_logger.LogInformation($"{srcFiles.Length} files found in source folder.");

			var filteredFiles = srcFiles.Where(w => filter(w)).ToList();
			_logger.LogInformation($"{filteredFiles.Count} files suffice passed filter.");

			for (int i = 0; i < filteredFiles.Count; i++)
			{
				string oldPath = filteredFiles[i];
				_logger.LogDebug($"Copying {i}/{filteredFiles.Count} file: '{oldPath}'.");

				var newPath = oldPath.Replace(sourceFolderPath, destFolderPath);
				_filesystemService.EnsureDirectory(newPath);
				_filesystemService.CopyFile(oldPath, newPath);

				_logger.LogDebug($"File '{oldPath}' has been copied to the '{newPath}'.");
			}

			_logger.LogInformation($"All files successfully copied.");
		}
	}
}
