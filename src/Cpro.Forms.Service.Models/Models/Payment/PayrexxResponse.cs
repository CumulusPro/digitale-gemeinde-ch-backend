namespace Cpro.Forms.Service.Models.Payment;

public class PayrexxResponse
{
    public string status { get; set; }
    public Datum[] data { get; set; }
}

public class Datum
{
    public int id { get; set; }
    public string status { get; set; }
    public string hash { get; set; }
    public string referenceId { get; set; }
    public string link { get; set; }
    public object[] invoices { get; set; }
    public bool preAuthorization { get; set; }
    public object[] fields { get; set; }
    public object[] psp { get; set; }
    public int amount { get; set; }
    public string currency { get; set; }
    public string vatRate { get; set; }
    public object sku { get; set; }
    public int createdAt { get; set; }
    public int requestId { get; set; }
}
