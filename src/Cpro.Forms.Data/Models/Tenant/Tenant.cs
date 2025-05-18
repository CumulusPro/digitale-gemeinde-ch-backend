using Peritos.Common.Data.BaseModels;

namespace Cpro.Forms.Data.Models.Tenant;

public class Tenant : Auditable
{
    public Guid Id { get; set; }
    public int TenantId { get; set; }
    public string TenantName { get; set; }
}
