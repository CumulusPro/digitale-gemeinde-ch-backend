namespace Cpro.Forms.Service.Models;

public class FieldRequest
{
    public int? Id { get; set; }
    public string DocumentTypeName { get; set; }
    public string Name { get; set; }
    public List<Fields> Fields { get; set; }
    public List<int>? designers { get; set; }
    public List<int>? processors { get; set; }
    public List<FormStatesConfig>? formStatesConfig { get; set; }
    public Header? header { get; set; }
    public Footer? footer { get; set; }
    public Rules rules { get; set; }
    public SubmissionConfig? submission { get; set; }
    public bool? showPreview { get; set; }
    public bool? useTenantDesign { get; set; }
    public DesignConfig designConfig { get; set; }
    public PaymentConfig paymentConfig { get; set; }
    public EmailNotificationConfig emailNotificationConfig { get; set; }
    public List<string>? tags { get; set; }

}


public class Fields
{
    public int? Id { get; set; }
    public int? SortOrder { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool? Hide { get; set; }
    public bool? DisplayInTaskUI { get; set; }
    public string Datatype { get; set; } = string.Empty;
    public string? LookupTableName { get; set; }
    public string? LookupValueFieldName { get; set; }
    public string? LookupFilterDataFieldName { get; set; }
    public string? LookupDescriptionFieldName { get; set; }
    public string? LookupGroupFieldName { get; set; }
    public bool? ReadOnly { get; set; }
    public string? Value { get; set; }
    public bool? IsRequired { get; set; }
    public string? RegularExpression { get; set; }
    public string? RegularExpressionMessage { get; set; }
    public int? ColumnIndex { get; set; }
    public string? Description { get; set; }
    public List<FileUploadAdditionalData>? FileUploadAdditionalData { get; set; }
    public List<LookupValues>? LookupValues { get; set; }
    public List<FieldRuleConfig>? fieldRulesConfig { get; set; }
    public string TabName { get; set; }
    public string? AddressFields { get; set; }
    public string? extras { get; set; }
}
