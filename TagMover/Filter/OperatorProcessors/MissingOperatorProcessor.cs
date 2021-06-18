using System;
using Microsoft.Extensions.Logging;
using TagMover.TagProcessors;

namespace TagMover.Filter.OperatorProcessors
{
	public class MissingOperatorProcessor : IOperatorProcessor
	{
		protected readonly ILogger<MissingOperatorProcessor> _logger;
		protected readonly ID3v2Processor _id3v2Processor;

		public MissingOperatorProcessor(
			ILogger<MissingOperatorProcessor> logger,
			ID3v2Processor id3v2Processor)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_id3v2Processor = id3v2Processor ?? throw new ArgumentNullException(nameof(id3v2Processor));
		}

		public Operators Operator => Operators.MISSING;

		public FilterFunc GetFilterFunction(string[] operatorArgs)
		{
			var fieldName = operatorArgs[0];

			return (filePath) =>
			{
				var fieldTag = _id3v2Processor.GetUserTag(filePath, fieldName);

				var success = fieldTag == null;
				if (success)
				{
					_logger.LogTrace($"'{filePath}': file will be copied, '{fieldName}' is missing");
				}
				else
				{
					_logger.LogTrace($"'{filePath}': file will be skipped, '{fieldName}' is '{String.Join("; ", fieldTag.Text)}'");
				}

				return success;
			};
		}
	}
}
