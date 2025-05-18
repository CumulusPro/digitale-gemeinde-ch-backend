using Cpro.Forms.Integration.Straatos.Configuration;
using Microsoft.Extensions.Logging;

namespace Cpro.Forms.Integration.Straatos.Services;

public interface IHttpService
{
    public Task<string> GetAsync(string url, string step = null, string token = null);
    public Task<string> PostAsync(string url, HttpContent content, string step = null);
    public Task<string> PutAsync(string url, HttpContent content);
}

public class HttpService : IHttpService
{
    private readonly HttpClient _httpClient;
    private readonly IStraatosConfiguration _configuration;
    private readonly ILogger<HttpService> _logger;

    public HttpService(IStraatosConfiguration configuration, ILogger<HttpService> logger)
    {
        _configuration = configuration;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("Authorization", _configuration.AuthToken);
        _logger = logger;
    }

    public async Task<string> GetAsync(string url, string step = null, string token = null)
    {
        if (!string.IsNullOrWhiteSpace(token)) {
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _httpClient.DefaultRequestHeaders.Add("Authorization", token);
        }
        var response = await _httpClient.GetAsync(url);
        return await HandleResponse(response, url);
    }

    public async Task<string> PostAsync(string url, HttpContent content, string step = null)
    {
        var response = await _httpClient.PostAsync(url, content);
        return await HandleResponse(response, url);
    }

    public async Task<string> PutAsync(string url, HttpContent content)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, url)
        {
            Content = content
        };

        var response = await _httpClient.SendAsync(request);
        return await HandleResponse(response, url);
    }

    private async Task<string> HandleResponse(HttpResponseMessage response, string url = null)
    {
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }
        else
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            // _logger.LogError($"HTTP request failed with status code {response.StatusCode}. Error: {errorMessage}. Url: {url}");
            throw new HttpRequestException($"HTTP request failed in api '{url}' with status code {response.StatusCode}. Error: {errorMessage}");
        }
    }
}