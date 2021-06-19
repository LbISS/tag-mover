using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using TagMover.Filter.OperatorProcessors;
using TagMover.Tag;

namespace TagMover.Filter
{
	public class FilterService : IFilterService
	{
		protected readonly ILogger<FilterService> _logger;
		protected readonly ITagsService _tagsService;
		protected readonly IEnumerable<IOperatorProcessor> _operatorsProcessors;

		public FilterService(
			ILogger<FilterService> logger,
			ITagsService tagsService,
			IEnumerable<IOperatorProcessor> operatorsProcessors)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_tagsService = tagsService ?? throw new ArgumentNullException(nameof(tagsService));
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
				filterFunction = (_, _) => true;
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

			return (filePath, tags) =>
			{
				if (excludeRegexp != null && excludeRegexp.IsMatch(filePath))
				{
					_logger.LogDebug($"'{filePath}': file will be skipped, passing exclude pattern.");
					return false;
				}

				if (includeRegexp != null && !includeRegexp.IsMatch(filePath))
				{
					_logger.LogDebug($"'{filePath}': file will be skipped, not passing include pattern.");
					return false;
				}

				return filterFunction(filePath, tags);
			};
		}

		protected IOperatorProcessor GetOperatorProcessor(Operators oper)
		{
			return _operatorsProcessors.Single(processor => processor.Operator == oper);
		}
	}
}
