namespace Cpro.Forms.Service.Models;

public class FormDefinitionResponse
{
    public int id { get; set; }
    public int documentTypeId { get; set; }
    public string documentTypeName { get; set; }
    public Formdefinition formDefinition { get; set; }
}

public class Formdefinition
{
    public string title { get; set; }
    public string formGuid { get; set; }
    public bool isPublished { get; set; }
    public List<Formfielddefinition> formFieldDefinitions { get; set; }
}

public class Formfielddefinition
{
    public int displayOrder { get; set; }
    public int columnIndex { get; set; }
    public string readOnlyTextBlock { get; set; }
    public bool isLongText { get; set; }
    public string fileUploadAdditionalDataKey { get; set; }
    public Field2 field { get; set; }
    public Fieldlookupdefinition fieldLookupDefinition { get; set; }
    public string fileUploadLabel { get; set; }
    public bool fileUploadMandatory { get; set; }

}

public class Field2
{
    public int id { get; set; }
    public string name { get; set; }
    public string label { get; set; }
    public string fieldType { get; set; }
    public bool mandatory { get; set; }
}

public class Fieldlookupdefinition
{
    public string lookupType { get; set; }
    public List<Lookupdefinition> lookupDefinitions { get; set; }
}

public class Lookupdefinition
{
    public int displayOrder { get; set; }
    public string displayName { get; set; }
    public string value { get; set; }
}
