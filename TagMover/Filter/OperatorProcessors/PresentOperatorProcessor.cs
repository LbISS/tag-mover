using System;
using Microsoft.Extensions.Logging;
using TagMover.TagProcessors;

namespace TagMover.Filter.OperatorProcessors
{
	public class PresentOperatorProcessor : IOperatorProcessor
	{
		protected readonly ILogger<PresentOperatorProcessor> _logger;
		protected readonly ID3v2Processor _id3v2Processor;

		public PresentOperatorProcessor(
			ILogger<PresentOperatorProcessor> logger,
			ID3v2Processor id3v2Processor)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_id3v2Processor = id3v2Processor ?? throw new ArgumentNullException(nameof(id3v2Processor));
		}

		public Operators Operator => Operators.PRESENT;

		public FilterFunc GetFilterFunction(string[] operatorArgs)
		{
			var fieldName = operatorArgs[0];

			return (filePath) =>
			{
				var fieldTag = _id3v2Processor.GetUserTag(filePath, fieldName);

				var success = fieldTag != null;
				if (success)
				{
					_logger.LogTrace($"'{filePath}': filter passed, '{fieldName}' is present('{String.Join("; ", fieldTag.Text)}')");
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
