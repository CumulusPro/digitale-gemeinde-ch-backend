using Microsoft.Extensions.Configuration;
using Peritos.Common.Configuration;

namespace Cpro.Forms.Integration.Storage.Configuration;

public interface IStorageConfiguration
{
    string AzureStorageConnectionString { get; set; }
    string ContainerName { get; set; }
    string Provider { get; set; }
    string OnPremStoragePath { get; set; }
}

public class StorageConfiguration : ConfigurationBase, IStorageConfiguration
{
    public StorageConfiguration(IConfiguration configuration) : base(configuration, "Storage")
    {

    }

    public string AzureStorageConnectionString { get; set; }
    public string ContainerName { get; set; }
    public string Provider { get; set; }
    public string OnPremStoragePath { get; set; }
}
