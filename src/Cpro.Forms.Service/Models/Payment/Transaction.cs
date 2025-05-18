namespace Cpro.Forms.Service.Models.Payment;

public class Transaction
{
    public string status { get; set; }
    public Datum1[] data { get; set; }
    public string message { get; set; }
}

public class Datum1
{
    public int id { get; set; }
    public string uuid { get; set; }
    public int amount { get; set; }
    public string referenceId { get; set; }
    public string time { get; set; }
    public string status { get; set; }
    public string lang { get; set; }
    public string psp { get; set; }
    public int pspId { get; set; }
    public string mode { get; set; }
    public Invoice2 invoice { get; set; }
    public Contact2 contact { get; set; }
    public object pageUuid { get; set; }
    public Payment2 payment { get; set; }
}

public class Invoice2
{
    public Product[] products { get; set; }
    public object discount { get; set; }
    public object shippingAmount { get; set; }
    public int? totalAmount { get; set; }
    public string currencyAlpha3 { get; set; }
    public Customfield[] customFields { get; set; }
}

public class Product
{
    public string name { get; set; }
    public int quantity { get; set; }
    public object sku { get; set; }
    public object vatRate { get; set; }
    public int amount { get; set; }
}

public class Customfield
{
    public string type { get; set; }
    public string name { get; set; }
    public string value { get; set; }
}

public class Contact2
{
    public int id { get; set; }
    public string uuid { get; set; }
    public string title { get; set; }
    public string firstname { get; set; }
    public string lastname { get; set; }
    public string company { get; set; }
    public string street { get; set; }
    public string zip { get; set; }
    public string place { get; set; }
    public string country { get; set; }
    public string countryISO { get; set; }
    public string phone { get; set; }
    public string email { get; set; }
    public string date_of_birth { get; set; }
    public string delivery_title { get; set; }
    public string delivery_firstname { get; set; }
    public string delivery_lastname { get; set; }
    public string delivery_company { get; set; }
    public string delivery_street { get; set; }
    public string delivery_zip { get; set; }
    public string delivery_place { get; set; }
    public string delivery_country { get; set; }
    public string delivery_countryISO { get; set; }
}

public class Payment2
{
    public string brand { get; set; }
}
