namespace TagMover.Filter
{
	interface IFilterService
	{
		FilterFunc GetFilterFunction(string filter);
	}
}
