namespace Cpro.Forms.Data.Models;

public class FormData
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string DocumentId { get; set; }
    public string Status { get; set; }
    public int TenantId { get; set; }
    public string FormId { get; set; }
    public string? TenantName { get; set; }
    public int Version { get; set; }
    public string? StorageUrl { get; set; }
    public DateTime SubmittedDate { get; set; }
}