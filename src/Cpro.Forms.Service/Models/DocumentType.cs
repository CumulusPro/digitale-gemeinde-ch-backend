namespace Cpro.Forms.Service.Models;

public class DocumentType
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public List<Field> Fields { get; set; }
    public Formdefinition FormDefinition { get; set; }
}

public class Field
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Label { get; set; }
    public string FieldType { get; set; }
    public int MaxLength { get; set; }
    public bool Mandatory { get; set; }
    public string FieldLookupType { get; set; }
}
