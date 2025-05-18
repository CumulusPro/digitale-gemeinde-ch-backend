namespace Cpro.Forms.Integration.SendGrid.Dto;

public class EmailDto
{
    public string Subject { get; set; }
    public Dictionary<string, string> PlaceHolders { get; set; }
    public Dictionary<byte[], string> AttachmentPlaceHolders { get; set; }
    public string HtmlContent { get; set; }
    public string Email { get; set; }
    public string ToEmail { get; set; }
}
