namespace Peritos.Common.Abstractions.Paging
{
    /// <summary>
    /// Represents the parameters for paging.
    /// </summary>
    public class PagingParameters
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 100;
        public string OrderBy { get; set; }
        public PagingParametersOrderByDirectionEnum OrderByDirection { get; set; }
    }

    public enum PagingParametersOrderByDirectionEnum
    {
        Ascending,
        Descending
    }
}
