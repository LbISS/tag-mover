namespace TagMover.Filter
{
	/// <summary>
	/// Service responsible for parsing incoming filter string
	/// </summary>
	interface IFilterService
	{
		/// <summary>
		/// Gets the filter function from passed filter string.
		/// </summary>
		/// <param name="filter">The filter string.</param>
		/// <returns></returns>
		FilterFunc GetFilterFunction(string filter);
	}
}
