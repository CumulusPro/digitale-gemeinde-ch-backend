using Cpro.Forms.Integration.Straatos.Services;
using Cpro.Forms.Service.Models.Payment;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Cpro.Forms.Service.Services;


public class PaymentService : IPaymentService
{
    private string _instance = "billing";
    private readonly HttpClient _httpClient;
    private readonly ILogger<HttpService> _logger;

    public PaymentService(ILogger<HttpService> logger)
    {
        _httpClient = new HttpClient();
        _logger = logger;
    }

    public async Task<string> CreatePaymentRequest(PaymentRequest paymentRequest)
    {
        if (paymentRequest == null)
        {
            return string.Empty;
        }
        string amount = Convert.ToInt32(Convert.ToDouble(paymentRequest?.amount) * 100).ToString();
        string currency = paymentRequest?.currency;
        string referenceId = paymentRequest?.referenceId;
        string successRedirectUrl = $"{paymentRequest.baseurl}/workflow-forms/{paymentRequest.tenantId}/{paymentRequest.formId}/form-success";
        string failedRedirectUrl = $"{paymentRequest.baseurl}/workflow-forms/{paymentRequest.tenantId}/{paymentRequest.formId}/form-error";
        string cancelRedirectUrl = $"{paymentRequest.baseurl}/workflow-forms/{paymentRequest.tenantId}/{paymentRequest.formId}/form-error";

        var query = $"amount={amount}&currency={currency}&referenceId={referenceId}" +
            $"&successRedirectUrl={WebUtility.UrlEncode(successRedirectUrl)}" +
            $"&failedRedirectUrl={WebUtility.UrlEncode(failedRedirectUrl)}" +
            $"&cancelRedirectUrl={WebUtility.UrlEncode(cancelRedirectUrl)}";


        _instance = string.IsNullOrWhiteSpace(paymentRequest.instance) ? _instance : paymentRequest.instance;
        var apiKey = paymentRequest.apikey;
            
        var apiSignature = BuildSignature(query, apiKey);
        var apiEndPoint = "https://api.pay.f4d.ch/v1.0/Gateway/?instance=" + _instance;

        var list = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("amount", amount),
                new KeyValuePair<string, string>("currency", currency),
                new KeyValuePair<string, string>("referenceId", referenceId),
                new KeyValuePair<string, string>("successRedirectUrl", successRedirectUrl),
                new KeyValuePair<string, string>("failedRedirectUrl", failedRedirectUrl),
                new KeyValuePair<string, string>("cancelRedirectUrl", cancelRedirectUrl)
            };

        list.Add(new KeyValuePair<string, string>("ApiSignature", apiSignature));

        var formContent = new FormUrlEncodedContent(list);

        var response = await _httpClient.PostAsync(apiEndPoint, formContent);

        var strResponse = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            // _logger.LogInformation($"apiKey: {apiKey}, instance= {_instance}, error: " + strResponse);
            throw new Exception(strResponse);
        }

        var payrexxResponse = JsonConvert.DeserializeObject<PayrexxResponse>(strResponse);

        _logger.LogInformation($"Payrexx response: {response.ReasonPhrase}");
        _logger.LogInformation($"Payrexx response: {await response.Content.ReadAsStringAsync()}");

        string responseMessage = payrexxResponse.data.FirstOrDefault()?.link;
        int id = payrexxResponse.data.FirstOrDefault()?.id ?? 0;

        return responseMessage;

    }

    public static string BuildSignature(string query, string client_secret)
    {
        Encoding ascii = Encoding.ASCII;

        HMACSHA256 hmac = new(ascii.GetBytes(client_secret));

        string calc_sig = Convert.ToBase64String(hmac.ComputeHash(ascii.GetBytes(query)));
        return calc_sig;
    }
}
