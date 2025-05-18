using Microsoft.Extensions.Configuration;
using Peritos.Common.Configuration;

namespace Cpro.Forms.Integration.Straatos.Configuration;

public interface IStraatosConfiguration
{
    string BaseUrl { get; set; }
    string AuthToken { get; set; }
    string GroupName { get; set; }
    string OrganizationId { get; set; }
    string WorkflowStepId { get; set; }
    string AzureStorageConnectionString { get; set; }
    string ContainerName { get; set; }
}

public class StraatosConfiguration : ConfigurationBase, IStraatosConfiguration
{
    public StraatosConfiguration(IConfiguration configuration) : base(configuration, "Straatos")
    {

    }

    public string BaseUrl { get; set; }
    public string AuthToken { get; set; }
    public string GroupName { get; set; }
    public string OrganizationId { get; set; }
    public string WorkflowStepId { get; set; }
    public string AzureStorageConnectionString { get; set; }
    public string ContainerName { get; set; }
}
