using System;
using Microsoft.Extensions.Logging;
using TagMover.Tag;

namespace TagMover.Filter.OperatorProcessors
{
	public class PresentOperatorProcessor : IOperatorProcessor
	{
		protected readonly ILogger<PresentOperatorProcessor> _logger;

		public PresentOperatorProcessor(
			ILogger<PresentOperatorProcessor> logger)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public Operators Operator => Operators.PRESENT;

		public FilterFunc GetFilterFunction(string[] operatorArgs)
		{
			var fieldName = operatorArgs[0];

			return (string filePath, FileTags tags) =>
			{
				var fieldTagValue = tags.ContainsKey(fieldName) ? tags[fieldName] : null;

				var success = fieldTagValue != null;
				if (success)
				{
					_logger.LogTrace($"'{filePath}': filter passed, '{fieldName}' is present('{String.Join("; ", fieldTagValue)}')");
				}
				else
				{
					_logger.LogTrace($"'{filePath}': filter not passed, '{fieldName}' is missing");
				}

				return success;
			};
		}
	}
}
