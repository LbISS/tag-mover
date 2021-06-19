using System;
using Microsoft.Extensions.Logging;
using TagMover.Tag;

namespace TagMover.Filter.OperatorProcessors
{
	public class MissingOperatorProcessor : IOperatorProcessor
	{
		protected readonly ILogger<MissingOperatorProcessor> _logger;

		public MissingOperatorProcessor(
			ILogger<MissingOperatorProcessor> logger)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public Operators Operator => Operators.MISSING;

		public FilterFunc GetFilterFunction(string[] operatorArgs)
		{
			var fieldName = operatorArgs[0];

			return (string filePath, FileTags tags) =>
			{
				var fieldTagValue = tags.ContainsKey(fieldName) ? tags[fieldName] : null;

				var success = fieldTagValue == null;
				if (success)
				{
					_logger.LogTrace($"'{filePath}': file will be copied, '{fieldName}' is missing");
				}
				else
				{
					_logger.LogTrace($"'{filePath}': file will be skipped, '{fieldName}' is '{String.Join("; ", fieldTagValue)}'");
				}

				return success;
			};
		}
	}
}
