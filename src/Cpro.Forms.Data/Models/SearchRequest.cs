using Peritos.Common.Abstractions.Paging;

namespace Cpro.Forms.Data.Models;

public class SearchRequest : PagingParameters
{
    public string Keyword { get; set; }
}

