namespace Cpro.Forms.Data.Models.User;

public class UserSearchRequest : SearchRequest
{
    public string? Email { get; set; }
    public string? FirstName { get; set; }
}
