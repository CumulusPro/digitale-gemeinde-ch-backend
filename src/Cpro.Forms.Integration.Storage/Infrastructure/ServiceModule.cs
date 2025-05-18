using Cpro.Forms.Integration.Storage.Configuration;
using Cpro.Forms.Integration.Storage.Services;
using Microsoft.Extensions.DependencyInjection;
using Peritos.Common.DependencyInjection;

namespace Cpro.Forms.Integration.Storage.Infrastructure;

public class ServiceModule : IServiceCollectionModule
{
    public void Load(IServiceCollection services)
    {
        services.AddSingleton<IStorageConfiguration, StorageConfiguration>();

        services.AddTransient<IAzureBlobService>(provider =>
        {
            var config = provider.GetRequiredService<IStorageConfiguration>();
            return config.Provider.Equals("Azure", StringComparison.OrdinalIgnoreCase)
                ? new AzureBlobService(config)
                : new OnPremStorageService(config);
        });

        services.AddWithTypeFilter(x => x.Name.EndsWith("Service"), ServiceCollectionLifetime.Transient);
    }
}
