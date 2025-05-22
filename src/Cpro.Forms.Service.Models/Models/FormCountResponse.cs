using Newtonsoft.Json;

namespace Cpro.Forms.Service.Models;
public class Request
{
     [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public string? Keyword { get; set; }
    public string? FormId { get; set; }
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public string? Status { get; set; }
    public int Page { get; set; } = 0;
    public int PageSize { get; set; } = 50;
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public string? OrderBy { get; set; } 
    [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
    public string? OrderByDirection { get; set; }
}

public class FormStatusNavigation
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int Count { get; set; }
    public Request Request { get; set; }
}

public class FormNavigation
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int Count { get; set; }
    public Request Request { get; set; }
    public List<FormStatusNavigation> FormNavigations { get; set; } = new List<FormStatusNavigation>();
}