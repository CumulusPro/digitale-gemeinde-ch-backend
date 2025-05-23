using System.Collections.Generic;

namespace Peritos.Common.Abstractions.Paging
{
   
    /// <summary>
    /// Represents a paginated response.
    /// </summary>
    public class PagingResponse<TItem>
    {
        public List<TItem> Data { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
