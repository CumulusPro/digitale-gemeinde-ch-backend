namespace Cpro.Forms.Data.Models;

public class FormTemplate
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int TenantId { get; set; }
    public string? TenantName { get; set; }
    public int Version { get; set; }
    public string? StorageUrl { get; set; }
}