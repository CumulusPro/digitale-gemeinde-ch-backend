namespace Peritos.Common.Abstractions
{
    /// <summary>
    /// Interface for the request context. This is used to pass information about the current request.
    /// </summary>
    public interface IRequestContext
    {
        bool IsAuthenticated { get; }
        int? UserId { get; }
        int? RoleId { get; }
        string UserEmail { get; set; }
        string? Token { get; }
    }
}
