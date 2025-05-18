using Microsoft.Extensions.DependencyInjection;
using Peritos.Common.DependencyInjection;

namespace Cpro.Forms.Service.Infrastructure
{
    public class ServiceModule : IServiceCollectionModule
    {
        public void Load(IServiceCollection services)
        {
            services.AddWithTypeFilter(x => x.Name.EndsWith("Service"), ServiceCollectionLifetime.Transient);
        }
    }
}
