using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using TagMover.Filter.OperatorProcessors;
using TagMover.TagProcessors;

namespace TagMover.Filter
{
	public class FilterService : IFilterService
	{
		protected readonly ILogger<FilterService> _logger;
		protected readonly ID3v2Processor _id3v2Processor;
		protected readonly IEnumerable<IOperatorProcessor> _operatorsProcessors;

		public FilterService(
			ILogger<FilterService> logger,
			ID3v2Processor id3v2Processor,
			IEnumerable<IOperatorProcessor> operatorsProcessors)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_id3v2Processor = id3v2Processor ?? throw new ArgumentNullException(nameof(id3v2Processor));
			_operatorsProcessors = operatorsProcessors ?? throw new ArgumentNullException(nameof(operatorsProcessors));
		}

		public FilterFunc GetFilterFunction(string filter, string includePattern = null, string excludePattern = null)
		{
			_logger.LogTrace($"Starting analyze of filter '{filter}'");

			FilterFunc filterFunction;
			Regex includeRegexp = !String.IsNullOrEmpty(includePattern) ? new Regex(includePattern, RegexOptions.Compiled) : null;
			Regex excludeRegexp = !String.IsNullOrEmpty(excludePattern) ? new Regex(excludePattern, RegexOptions.Compiled) : null;

			if (String.IsNullOrEmpty(filter))
			{
				filterFunction = (s) => true;
			}
			else
			{
				//TODO: Fully functional filter to LINQ converter
				var parts = filter.Split(' ');

				if (parts.Length < 2)
				{
					throw new InvalidOperationException($"Unsupported filter syntax.");
				}

				if (parts[1].Equals(Operators.MISSING.ToString()))
				{
					_logger.LogTrace($"'{Operators.MISSING}' operator found.");

					filterFunction = GetOperatorProcessor((Operators)Enum.Parse(typeof(Operators), parts[1])).GetFilterFunction(parts);
				}
				else if (parts[1].Equals(Operators.PRESENT.ToString()))
				{
					_logger.LogTrace($"'{Operators.PRESENT}' operator found.");

					filterFunction = GetOperatorProcessor((Operators)Enum.Parse(typeof(Operators), parts[1])).GetFilterFunction(parts);
				}
				else
					throw new InvalidOperationException($"Unsupported filter operator '{parts[1]}'.");
			}

			return (filePath) =>
			{
				if (excludeRegexp != null && excludeRegexp.IsMatch(filePath))
				{
					_logger.LogDebug($"'{filePath}': file will be skipped, passing exclude pattern.");
					return false;
				}

				if (!filterFunction(filePath))
				{
					return false;
				}

				if (includeRegexp != null && !includeRegexp.IsMatch(filePath))
				{
					_logger.LogDebug($"'{filePath}': file will be skipped, not passing include pattern.");
					return false;
				}

				return true;
			};
		}

		protected IOperatorProcessor GetOperatorProcessor(Operators oper)
		{
			return _operatorsProcessors.Single(processor => processor.Operator == oper);
		}
	}
}
