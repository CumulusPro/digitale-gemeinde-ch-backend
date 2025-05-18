using Peritos.Common.Data.BaseModels;

namespace Cpro.Forms.Data.Models.User;

public class User : Auditable
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Email { get; set; }
    public Role Role { get; set; }
    public int TenantId { get; set; }
}

public enum Role
{
    Admin,
    Designer,
    Processor
}
