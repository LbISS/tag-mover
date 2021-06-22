using System;
using System.Text.RegularExpressions;
using Antlr4.Runtime.Misc;
using Microsoft.Extensions.Logging;
using TagMover.Tag;
using static FilterQueryParser;

namespace TagMover.Filter
{
	public class AntlrFilterQueryVisitor : FilterQueryBaseVisitor<FilterFunc>
	{
		protected readonly ILogger<AntlrFilterQueryVisitor> _logger;

		public AntlrFilterQueryVisitor(
			ILogger<AntlrFilterQueryVisitor> logger)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public override FilterFunc VisitAndOperator([NotNull] AndOperatorContext context)
		{
			return (filePath, tags) =>
			{
				return Visit(context.expression(0))(filePath, tags)
						&& Visit(context.expression(1))(filePath, tags);
			};
		}

		public override FilterFunc VisitOrOperator([NotNull] OrOperatorContext context)
		{
			return (filePath, tags) =>
			{
				return Visit(context.expression(0))(filePath, tags)
						|| Visit(context.expression(1))(filePath, tags);
			};
		}

		public override FilterFunc VisitBracketOperator([NotNull] BracketOperatorContext context)
		{
			return (filePath, tags) =>
			{
				return Visit(context.expression())(filePath, tags);
			};
		}

		public override FilterFunc VisitNotOperator([NotNull] NotOperatorContext context)
		{
			return (filePath, tags) =>
			{
				return !Visit(context.expression())(filePath, tags);
			};
		}

		public override FilterFunc VisitMissingOperator([NotNull] MissingOperatorContext context)
		{
			var fieldName = context.FIELD().GetText();

			return (filePath, tags) =>
			{
				var fieldTagValue = tags.ContainsKey(fieldName) ? tags[fieldName] : null;

				var success = fieldTagValue == null;

				_logger.LogTrace(GetLogPrefix(filePath, context, fieldName, fieldTagValue) + $"MISSING: {success}");

				return success;
			};
		}
		public override FilterFunc VisitPresentOperator([NotNull] PresentOperatorContext context)
		{
			var fieldName = context.FIELD().GetText();

			return (filePath, tags) =>
			{
				var fieldTagValue = tags.ContainsKey(fieldName) ? tags[fieldName] : null;

				var success = fieldTagValue != null;

				_logger.LogTrace(GetLogPrefix(filePath, context, fieldName, fieldTagValue) + $"PRESENT: {success}");

				return success;
			};
		}

		public override FilterFunc VisitHasOperator([NotNull] HasOperatorContext context)
		{
			var fieldName = context.FIELD().GetText();
			var value = context.STRING().GetText();

			return (filePath, tags) =>
			{
				var fieldTagValue = tags.ContainsKey(fieldName) ? tags[fieldName] : null;

				var success = fieldTagValue.Contains(value);

				_logger.LogTrace(GetLogPrefix(filePath, context, fieldName, fieldTagValue) + $"HAS {value}: {success}");

				return success;
			};
		}

		public override FilterFunc VisitIsOperator([NotNull] IsOperatorContext context)
		{
			var fieldName = context.FIELD().GetText();
			var value = context.STRING().GetText();

			return (filePath, tags) =>
			{
				var fieldTagValue = tags.ContainsKey(fieldName) ? tags[fieldName] : null;

				var success = fieldTagValue.Equals(value, StringComparison.CurrentCulture);

				_logger.LogTrace(GetLogPrefix(filePath, context, fieldName, fieldTagValue) + $"IS {value}: {success}");

				return success;
			};
		}

		public override FilterFunc VisitGreaterOperator([NotNull] GreaterOperatorContext context)
		{
			var fieldName = context.FIELD().GetText();
			var strNumber = context.NUMBER().GetText();

			var parseNumber = double.TryParse(strNumber, out double value);
			if (!parseNumber)
			{
				throw new InvalidOperationException($"Cannot parse filter: {fieldName}: {strNumber} not a number");
			}

			return (filePath, tags) =>
			{
				var fieldTagValue = tags.ContainsKey(fieldName) ? tags[fieldName] : null;

				var parseTagValue = double.TryParse(fieldTagValue, out double tagValue);
				if (!parseTagValue)
				{
					throw new InvalidOperationException($"{filePath}: Cannot parse {fieldName}: {fieldTagValue} not a number");
				}

				var success = tagValue > value;

				_logger.LogTrace(GetLogPrefix(filePath, context, fieldName, fieldTagValue) + $"GREATER THAN {value}: {success}");

				return success;
			};
		}

		public override FilterFunc VisitEqualOperator([NotNull] EqualOperatorContext context)
		{
			var fieldName = context.FIELD().GetText();
			var strNumber = context.NUMBER().GetText();

			var parseNumber = double.TryParse(strNumber, out double value);
			if (!parseNumber)
			{
				throw new InvalidOperationException($"Cannot parse filter: {fieldName}: {strNumber} not a number");
			}

			return (filePath, tags) =>
			{
				var fieldTagValue = tags.ContainsKey(fieldName) ? tags[fieldName] : null;

				var parseTagValue = double.TryParse(fieldTagValue, out double tagValue);
				if (!parseTagValue)
				{
					throw new InvalidOperationException($"{filePath}: Cannot parse {fieldName}: {fieldTagValue} not a number");
				}

				var success = tagValue == value;

				_logger.LogTrace(GetLogPrefix(filePath, context, fieldName, fieldTagValue) + $"EQUAL {value}: {success}");

				return success;
			};
		}

		public override FilterFunc VisitLessOperator([NotNull] LessOperatorContext context)
		{
			var fieldName = context.FIELD().GetText();
			var strNumber = context.NUMBER().GetText();

			var parseNumber = double.TryParse(strNumber, out double value);
			if (!parseNumber)
			{
				throw new InvalidOperationException($"Cannot parse filter: {fieldName}: {strNumber} not a number");
			}

			return (filePath, tags) =>
			{
				var fieldTagValue = tags.ContainsKey(fieldName) ? tags[fieldName] : null;

				var parseTagValue = double.TryParse(fieldTagValue, out double tagValue);
				if (!parseTagValue)
				{
					throw new InvalidOperationException($"{filePath}: Cannot parse {fieldName}: {fieldTagValue} not a number");
				}

				var success = tagValue < value;

				_logger.LogTrace(GetLogPrefix(filePath, context, fieldName, fieldTagValue) + $"LESS THAN {value}: {success}");

				return success;
			};
		}

		public override FilterFunc VisitMatchesOperator([NotNull] MatchesOperatorContext context)
		{
			var fieldName = context.FIELD().GetText();
			var regexStr = context.REGEX().GetText();

			Regex regex;
			try
			{
				regex = new Regex(regexStr);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException($"Cannot parse {regexStr} as regex", ex);
			}

			return (filePath, tags) =>
			{
				var fieldTagValue = tags.ContainsKey(fieldName) ? tags[fieldName] : null;

				var success = regex.IsMatch(fieldTagValue);

				_logger.LogTrace(GetLogPrefix(filePath, context, fieldName, fieldTagValue) + $"MATCHES {regexStr}: {success}");

				return success;
			};
		}

		protected string GetLogPrefix(string filePath, ExpressionContext context, string fieldName, object fieldValue)
		{
			return $"'{filePath}': '{fieldName}' value is '{fieldValue}'. ";
		}
	}
}
