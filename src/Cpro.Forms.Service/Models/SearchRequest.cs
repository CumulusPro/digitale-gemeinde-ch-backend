using Peritos.Common.Abstractions.Paging;

namespace Cpro.Forms.Service.Models;

public class SearchRequest : PagingParameters
{
    public string? Keyword { get; set; } 
}

