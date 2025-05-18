using Cpro.Forms.Service.Models.Enums;

namespace Cpro.Forms.Service.Models.User;

public class UserResponse
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Email { get; set; }
    public Role Role { get; set; }
    public int TenantId { get; set; }
}
