using System.Collections.Generic;

namespace Peritos.Common.Abstractions.Paging
{

    /// <summary>
    /// Represents a paginated response containing a subset of data items along with pagination metadata.
    /// </summary>
    /// <typeparam name="TItem">The type of the items included in the response.</typeparam>
    public class PagingResponse<TItem>
    {
        /// <summary>
        /// Gets or sets the list of items returned for the current page.
        /// </summary>
        public List<TItem> Data { get; set; }

        /// <summary>
        /// Gets or sets the total number of items available across all pages.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Gets or sets the current page number.
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Gets or sets the number of items included per page.
        /// </summary>
        public int PageSize { get; set; }
    }
}
