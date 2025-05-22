namespace Cpro.Forms.Service.Models;

public class FormData
{
    public string id { get; set; }
    public string Name { get; set; }
    public string DocumentId { get; set; }
    public string Status { get; set; }
    public int TenantId { get; set; }
    public string FormId { get; set; }
    public string TenantName { get; set; }
    public DateTime SubmittedDate { get; set; }
}