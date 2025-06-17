using Cpro.Forms.Integration.Straatos.Configuration;
using Microsoft.Extensions.Logging;

namespace Cpro.Forms.Integration.Straatos.Services;

/// <summary>
/// Interface for HTTP service operations with Straatos API.
/// </summary>
public interface IHttpService
{
    /// <summary>
    /// Performs an HTTP GET request to the specified URL.
    /// </summary>
    /// <param name="url">The URL to send the GET request to</param>
    /// <param name="step">Optional step identifier for logging</param>
    /// <param name="token">Optional authentication token</param>
    /// <returns>The response content as a string</returns>
    public Task<string> GetAsync(string url, string step = null, string token = null);
    
    /// <summary>
    /// Performs an HTTP POST request to the specified URL.
    /// </summary>
    /// <param name="url">The URL to send the POST request to</param>
    /// <param name="content">The HTTP content to send</param>
    /// <param name="step">Optional step identifier for logging</param>
    /// <returns>The response content as a string</returns>
    public Task<string> PostAsync(string url, HttpContent content, string step = null);
    
    /// <summary>
    /// Performs an HTTP PUT request to the specified URL.
    /// </summary>
    /// <param name="url">The URL to send the PUT request to</param>
    /// <param name="content">The HTTP content to send</param>
    /// <returns>The response content as a string</returns>
    public Task<string> PutAsync(string url, HttpContent content);
}

/// <summary>
/// Service for handling HTTP operations with Straatos API, including authentication and error handling.
/// </summary>
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

    /// <summary>
    /// Performs an HTTP GET request to the specified URL with optional authentication token override.
    /// </summary>
    /// <param name="url">The URL to send the GET request to</param>
    /// <param name="step">Optional step identifier for logging</param>
    /// <param name="token">Optional authentication token to override default</param>
    /// <returns>The response content as a string</returns>
    /// <exception cref="HttpRequestException">Thrown when the HTTP request fails</exception>
    public async Task<string> GetAsync(string url, string step = null, string token = null)
    {
        if (!string.IsNullOrWhiteSpace(token)) {
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _httpClient.DefaultRequestHeaders.Add("Authorization", token);
        }
        var response = await _httpClient.GetAsync(url);
        return await HandleResponse(response, url);
    }

    /// <summary>
    /// Performs an HTTP POST request to the specified URL.
    /// </summary>
    /// <param name="url">The URL to send the POST request to</param>
    /// <param name="content">The HTTP content to send</param>
    /// <param name="step">Optional step identifier for logging</param>
    /// <returns>The response content as a string</returns>
    /// <exception cref="HttpRequestException">Thrown when the HTTP request fails</exception>
    public async Task<string> PostAsync(string url, HttpContent content, string step = null)
    {
        var response = await _httpClient.PostAsync(url, content);
        return await HandleResponse(response, url);
    }

    /// <summary>
    /// Performs an HTTP PUT request to the specified URL.
    /// </summary>
    /// <param name="url">The URL to send the PUT request to</param>
    /// <param name="content">The HTTP content to send</param>
    /// <returns>The response content as a string</returns>
    /// <exception cref="HttpRequestException">Thrown when the HTTP request fails</exception>
    public async Task<string> PutAsync(string url, HttpContent content)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, url)
        {
            Content = content
        };

        var response = await _httpClient.SendAsync(request);
        return await HandleResponse(response, url);
    }

    /// <summary>
    /// Handles HTTP response and throws exceptions for unsuccessful status codes.
    /// </summary>
    /// <param name="response">The HTTP response message</param>
    /// <param name="url">Optional URL for error logging</param>
    /// <returns>The response content as a string</returns>
    /// <exception cref="HttpRequestException">Thrown when the response status code indicates failure</exception>
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