namespace Cpro.Forms.Service.Models;

public class FormRequest
{
    public long RequestId { get; set; }
    public string Municipality { get; set; }
    public string? Category { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Address { get; set; }
    public DateTime SubmissionDateTime { get; set; }
    public byte[] Image { get; set; }
}
