using Cpro.Forms.Service.Models.Enums;

namespace Cpro.Forms.Service.Models.User;

public class UserSearchRequest : SearchRequest
{
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public int? TenantId { get; set; }
    public Role? Role { get; set; }

}
