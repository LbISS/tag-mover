namespace TagMover.Filter
{
	interface IFilterService
	{
		FilterFunc GetFilterFunction(string filter, string includePattern = null, string excludePattern = null);
	}
}
