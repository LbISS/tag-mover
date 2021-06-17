namespace TagMover.Filter.OperatorProcessors
{
	public interface IOperatorProcessor
	{
		public Operators Operator { get; }
		public FilterFunc GetFilterFunction(string[] operatorArgs);
	}
}
