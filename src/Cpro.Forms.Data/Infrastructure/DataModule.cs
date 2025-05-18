using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Peritos.Common.Data.Configuration;
using Peritos.Common.DependencyInjection;

namespace Cpro.Forms.Data.Infrastructure;

public class DataModule : IServiceCollectionModule
{
    public void Load(IServiceCollection services)
    {
        services.AddWithTypeFilter(x => x.Name.EndsWith("Repository"), ServiceCollectionLifetime.Transient);
        services.AddDbContext<SqlContext>((serviceProvider, options) =>
        {
            options.UseSqlServer(serviceProvider.GetRequiredService<IDatabaseConfig>().ConnectionString);
        });
        // services.AddTransient<IStartupFilter, MigrationStartupFilter<SqlContext>>();

    }
}
