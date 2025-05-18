using System.Collections.Generic;

namespace Peritos.Common.Api.Paging
{
    public class PagingResponse<TItem>
    {
        public List<TItem> Items { get; set; }
        public int TotalCount { get; set; }
    }
}
