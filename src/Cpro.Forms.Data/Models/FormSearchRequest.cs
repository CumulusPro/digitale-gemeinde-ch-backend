namespace Cpro.Forms.Data.Models;

public class FormSearchRequest : SearchRequest
{
    public string Keyword { get; set; }
    public string? FormId { get; set; } 
    public string? Status { get; set; } 
}