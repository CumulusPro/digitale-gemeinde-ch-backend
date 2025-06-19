namespace Peritos.Common.Abstractions.Paging
{
    /// <summary>
    /// Represents the parameters for paging, including page number, page size, and sorting options.
    /// </summary>
    public class PagingParameters
    {
        /// <summary>
        /// Gets or sets the page number to retrieve. Defaults to 1.
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Gets or sets the number of items to retrieve per page. Defaults to 100.
        /// </summary>
        public int PageSize { get; set; } = 100;

        /// <summary>
        /// Gets or sets the name of the field to sort by.
        /// </summary>
        public string OrderBy { get; set; }

        /// <summary>
        /// Gets or sets the sort direction for the results (ascending or descending).
        /// </summary>
        public PagingParametersOrderByDirectionEnum OrderByDirection { get; set; }
    }

    /// <summary>
    /// Specifies the direction in which to sort paginated results.
    /// </summary>
    public enum PagingParametersOrderByDirectionEnum
    {
        /// <summary>
        /// Sort results in ascending order (e.g., A-Z or 0-9).
        /// </summary>
        Ascending,

        /// <summary>
        /// Sort results in descending order (e.g., Z-A or 9-0).
        /// </summary>
        Descending
    }
}
