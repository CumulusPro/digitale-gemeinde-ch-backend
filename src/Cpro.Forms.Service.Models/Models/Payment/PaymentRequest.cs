namespace Cpro.Forms.Service.Models.Payment;

public class PaymentRequest
{
    public string amount { get; set; }
    public string currency { get; set; }
    public string referenceId { get; set; }
    public string formId { get; set; }
    public int? tenantId { get; set; }
    public string baseurl { get; set; }
    public string apikey { get; set; }
    public string instance { get; set; }
}
