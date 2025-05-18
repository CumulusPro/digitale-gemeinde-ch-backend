namespace Peritos.Common.Abstractions
{
    public interface IRequestContext
    {
        bool IsAuthenticated { get; }
        int? UserId { get; }
        int? RoleId { get; }
        string UserEmail { get; set; }
        string? Token { get; }
    }
}
