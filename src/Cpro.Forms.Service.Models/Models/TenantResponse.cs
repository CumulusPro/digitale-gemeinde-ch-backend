namespace Cpro.Forms.Service.Models;

public class TenantResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Organisation Organisation { get; set; }
}

public class Organisation
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
