using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using TagMover.Filter.OperatorProcessors;

namespace TagMover.Filter
{
	public class FilterService : IFilterService
	{
		protected readonly ILogger<FilterService> _logger;
		protected readonly IEnumerable<IOperatorProcessor> _operatorsProcessors;

		public FilterService(
			ILogger<FilterService> logger,
			IEnumerable<IOperatorProcessor> operatorsProcessors)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_operatorsProcessors = operatorsProcessors ?? throw new ArgumentNullException(nameof(operatorsProcessors));
		}

		public FilterFunc GetFilterFunction(string filter)
		{
			_logger.LogTrace($"Starting analyzing filter '{filter}'");

			FilterFunc filterFunction;

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

			return filterFunction;
		}

		protected IOperatorProcessor GetOperatorProcessor(Operators oper)
		{
			return _operatorsProcessors.Single(processor => processor.Operator == oper);
		}
	}
}
