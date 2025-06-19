using System.Collections.Generic;

namespace Peritos.Common.Api.Paging
{
    /// <summary>
    /// Represents a paginated response containing a subset of data items and the total count of items available.
    /// </summary>
    /// <typeparam name="TItem">The type of the items being returned in the paginated response.</typeparam>
    public class PagingResponse<TItem>
    {
        /// <summary>
        /// Gets or sets the list of items on the current page.
        /// </summary>
        public List<TItem> Items { get; set; }

        /// <summary>
        /// Gets or sets the total number of items available across all pages.
        /// </summary>
        public int TotalCount { get; set; }
    }
}
