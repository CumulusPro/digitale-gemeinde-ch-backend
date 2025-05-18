using Microsoft.Extensions.DependencyInjection;
using Peritos.Common.DependencyInjection;

namespace Cpro.Forms.Integration.SendGrid.Infrastructure;

public class SendGridModule : IServiceCollectionModule
{
    public void Load(IServiceCollection services)
    {
        services.AddWithTypeFilter(x => x.Name.EndsWith("Service"), ServiceCollectionLifetime.Transient);
    }
}
