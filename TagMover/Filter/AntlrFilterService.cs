using System;
using Antlr4.Runtime;
using Microsoft.Extensions.Logging;

namespace TagMover.Filter
{
	/// <summary>
	/// Service to read passed filter string and convert to filter function based on Antlr
	/// </summary>
	/// <seealso cref="TagMover.Filter.IFilterService" />
	public class AntlrFilterService : IFilterService
	{
		protected readonly ILogger<AntlrFilterService> _logger;
		protected readonly AntlrFilterQueryVisitor _antlrFilterQueryVisitor;

		public AntlrFilterService(
			ILogger<AntlrFilterService> logger,
			AntlrFilterQueryVisitor antlrFilterQueryVisitor)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_antlrFilterQueryVisitor = antlrFilterQueryVisitor ?? throw new ArgumentNullException(nameof(antlrFilterQueryVisitor));
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
				AntlrInputStream input = new AntlrInputStream(filter);
				ITokenSource lexer = new FilterQueryLexer(input);
				ITokenStream tokenStream = new CommonTokenStream(lexer);
				FilterQueryParser parser = new FilterQueryParser(tokenStream);

				var rootExpression = parser.query();
				filterFunction = _antlrFilterQueryVisitor.Visit(rootExpression);
			}

			_logger.LogTrace($"Finish analyzing filter '{filter}'");

			return filterFunction;
		}
	}
}
