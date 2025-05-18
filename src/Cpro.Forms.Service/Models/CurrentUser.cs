namespace Cpro.Forms.Service.Models;
public class CurrentUser
{
    public int Id { get; set; }
    public string UniqueId { get; set; }
    public string GivenName { get; set; }
    public string Surname { get; set; }
    public string B2CObjectId { get; set; }
    public List<string> Emails { get; set; }
}