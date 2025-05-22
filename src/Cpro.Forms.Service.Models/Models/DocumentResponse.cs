using Newtonsoft.Json;

namespace Cpro.Forms.Service.Models;

public class DocumentResponse
{
    public int? Id { get; set; }
    public string DocumentTypeName { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public List<int>? designers { get; set; }
    public List<int>? processors { get; set; }
    public List<FormStatesConfig>? formStatesConfig { get; set; }
    public List<IndexField> Fields { get; set; }
    public bool ShowSubmit { get; set; } = true;
    public Header header { get; set; }
    public Footer footer { get; set; }
    public Rules rules { get; set; }
    public bool? showPreview { get; set; }
    public bool? useTenantDesign { get; set; }
    public SubmissionConfig submission { get; set; }
    public DesignConfig designConfig { get; set; }
    public PaymentConfig paymentConfig { get; set; }
    public EmailNotificationConfig emailNotificationConfig { get; set; }
    public bool? isFormConfigDisabled { get; set; }
    public int? formRetentionDays { get; set; }
    public int? formDataRetentionDays { get; set; }
    public bool? IsActive { get; set; }
    public string State { get; set; }
    public List<string>? tags { get; set; }
}

public class PaymentConfig
{
    public string? apiKey { get; set; }
    public string? instance { get; set; }
    public string? paymentService { get; set; }
    public double? amount { get; set; }
    public string? currency { get; set; }
    public int? isEnabled { get; set; }
    public PaymentCondition? PaymentCondition { get; set; }
}

public class EmailNotificationConfig
{
    public string? emailsBody { get; set; }
    public string? emailsSubject { get; set; }
    public string? emailsTo { get; set; }
    public bool? notificationEnabled { get; set; }
    public string? baseurl { get; set; }
}

public class PaymentCondition
{
    [JsonProperty("operator")]
    public string? operatorField { get; set; }
    public string? conditionField { get; set; }
    public string? conditionValue { get; set; }
}

public class DesignConfig
{
    public string? fieldTemplateName { get; set; }
    public string? fieldFontStyle { get; set; }
    public string? fieldLabelFontColor { get; set; }
    public string? fieldLabelFontSize { get; set; }
    public string? formPrimaryColor { get; set; }
    public string? formPrimaryContrastColor { get; set; }
    public string? formBgColor { get; set; }
    public string? formFontStyle { get; set; }
    public string? formFontColor { get; set; }
    public string? formTitleFontSize { get; set; }
    public string? formTitleFontColor { get; set; }
    public string? fieldBorderColor { get; set; }
    public string? fieldBorderColorError { get; set; }
    public string? stepBackBtnBorderColor { get; set; }
    public string? stepBackBtnBgColor { get; set; }
    public string? stepBackBtnFontColor { get; set; }
    public string? stepBackBtnFontSize { get; set; }
    public string? stepNextBtnBorderColor { get; set; }
    public string? stepNextBtnBgColor { get; set; }
    public string? stepNextBtnFontColor { get; set; }
    public string? stepNextBtnFontSize { get; set; }
    public string? stepBgColorActive { get; set; }
    public string? stepFontColorActive { get; set; }
    public string? stepBgColorInActive { get; set; }
    public string? stepFontColorInActive { get; set; }
}

public class Header
{
    public bool showHeader { get; set; }
    public string layoutType { get; set; }
    public string? content { get; set; }
    public string? tabName { get; set; }
    public string? favicon { get; set; }
    public Layoutprops layoutProps { get; set; }
}

public class Layoutprops
{
    public string? logoUrl { get; set; }
}

public class Footer
{
    public bool showFooter { get; set; }
    public string? content { get; set; }
    public string layoutType { get; set; }
    public Layoutprops1 layoutProps { get; set; }
}

public class Layoutprops1
{
    public string? copyRightText { get; set; }
}

public class HeaderSettings
{
    public string Title { get; set; }
    public string Logo { get; set; }
}

public class FooterSettings
{
    public string? CopyrightText { get; set; }
    public string Title { get; set; }
    public string Address { get; set; }
}

public class IndexField
{
    public int? Id { get; set; }
    public int SortOrder { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public bool Hide { get; set; }
    public bool DisplayInTaskUI { get; set; }
    public string Datatype { get; set; }
    public string LookupTableName { get; set; }
    public string LookupValueFieldName { get; set; }
    public string LookupFilterDataFieldName { get; set; }
    public string LookupDescriptionFieldName { get; set; }
    public string LookupGroupFieldName { get; set; }
    public bool? ReadOnly { get; set; }
    public string Value { get; set; }
    public bool? IsRequired { get; set; }
    public string RegularExpression { get; set; }
    public string RegularExpressionMessage { get; set; }
    public int? ColumnIndex { get; set; }
    public string Description { get; set; }
    public FileUploadAdditionalData FileUploadAdditionalData { get; set; }
    public List<LookupValues> LookupValues { get; set; }
    public List<FieldRuleConfig>? fieldRulesConfig { get; set; }
    public string TabName { get; set; }
    public string? extras { get; set; }

}

public class LookupValues
{
    public int displayOrder { get; set; }
    public string displayValue { get; set; }
    public string value { get; set; }
}

public class FileUploadAdditionalData
{
    public string key { get; set; }
    public string base64File { get; set; }
    public string name { get; set; }
    public string type { get; set; }
}

public class Rules
{
    public List<FieldTrigger> fieldTriggers { get; set; }
    public List<Function> functions { get; set; }
}

public class FieldTrigger
{
    public string Name { get; set; }
    public List<ChangeFunction> PreFunction { get; set; } // Empty lists, can be specified more depending on the actual use
    public List<ChangeFunction> PostFunction { get; set; } // Empty lists, can be specified more depending on the actual use
    public List<ChangeFunction> ChangeFunction { get; set; }
}

public class ChangeFunction
{
    public string Name { get; set; }
    public int Order { get; set; }
}

public class Function
{
    public string Name { get; set; }
    public List<Rule> Rules { get; set; }
}

public class Rule
{
    public int? Order { get; set; }
    public string Type { get; set; } // This can be further refined by using a strategy pattern or inheritance for different rule types
    public string? Field { get; set; }
    public string? Attribute { get; set; }
    public Value? Value { get; set; }
    public string? ConditionOperator { get; set; }
    public ConditionValue? ConditionValue1 { get; set; } // This could be a complex type, depending on the rule
    public object? ConditionValue2 { get; set; }
    public List<ConditionRule>? Then { get; set; }
    public List<ConditionRule>? Else { get; set; }
}

public class ConditionRule
{
    public int? Order { get; set; }
    public string? Type { get; set; } // This can be further refined by using a strategy pattern or inheritance for different rule types
    public string? Field { get; set; }
    public string? Message { get; set; }
    public string? Attribute { get; set; }
    public string? MessageType { get; set; }
    public object? Value { get; set; }
}

public class Value
{
    public int? Order { get; set; }
    public string? Type { get; set; }
    public string? ApiName { get; set; }
    public APIBody? Body { get; set; }
    public string? Field { get; set; }
    public string? Attribute { get; set; }
}

public class APIBody
{
    public FieldValue? fieldValue { get; set; }
}

public class FieldValue
{
    public string? Type { get; set; }
    public string? field { get; set; }
    public string? attribute { get; set; }
}

public class ConditionValue
{
    public string? Type { get; set; }
    public string? Expression { get; set; }
    public string? Column { get; set; }
    public string? Field { get; set; }
    public string? Attribute { get; set; }
}

public class SubmissionConfig
{
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
    public string? SuccessUrl { get; set; }
    public string? ErrorUrl { get; set; }
}

public class FieldRuleConfig
{
    public string Attribute { get; set; }
    public string AttributeValue { get; set; }
    public string Operator { get; set; }
    public string ConditionField { get; set; }
    public string ConditionValue { get; set; }
}
