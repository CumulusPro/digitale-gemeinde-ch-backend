namespace Cpro.Forms.Service.Models;

public class CreateFormTemplateRequest 
{
    public string Name { get; set; }
    public int? TenantId { get; set; }
    public List<Fields> Fields { get; set; }
    public Header? header { get; set; }
    public Footer? footer { get; set; }
    public Rules rules { get; set; }
    public SubmissionConfig? submission { get; set; }
    public bool? showPreview { get; set; }
    public bool? useTenantDesign { get; set; }
    public DesignConfig designConfig { get; set; }
    public PaymentConfig paymentConfig { get; set; }
}

