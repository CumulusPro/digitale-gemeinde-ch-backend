namespace Cpro.Forms.Service.Models;

public class GetCategoryApiResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class CreateDocumentTypeResponse
{
    public string Name { get; set; }
    public int Id { get; set; }
    public string Category { get; set; }
}

public class CreateFormDefinitionResponse
{
    public int id { get; set; }
    public int documentTypeId { get; set; }
    public string formGuid { get; set; }
    public string title { get; set; }
    public string documentTypeName { get; set; }
}
