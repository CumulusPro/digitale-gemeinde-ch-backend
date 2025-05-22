namespace Cpro.Forms.Service.Models;

public class CreateFormDefinitionRequest 
{
    public bool isPublished { get; set; }
    public string title { get; set; }
    public List<Formfielddefinition2> formFieldDefinitions { get; set; }
}

public class Formfielddefinition2
{
    public int displayOrder { get; set; }
    public int columnIndex { get; set; }
    public string? readOnlyTextBlock { get; set; }
    public bool? isLongText { get; set; }
    public string? fileUploadAdditionalDataKey { get; set; }
    public int fieldId { get; set; }
    public Fieldlookupdefinition? fieldLookupDefinition { get; set; }
    public string? fileUploadLabel { get; set; }
    public bool? fileUploadMandatory { get; set; }
    public bool Mandatory { get; set; }
}
