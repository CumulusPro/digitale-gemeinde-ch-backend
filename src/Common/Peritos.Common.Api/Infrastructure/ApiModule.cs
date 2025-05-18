using Microsoft.Extensions.DependencyInjection;
using Peritos.Common.Api.ApplicationInsights;
using Peritos.Common.DependencyInjection;

namespace Peritos.Common.Api.Infrastructure
{
    public class ApiModule : IServiceCollectionModule
    {
        public void Load(IServiceCollection services)
        {
            services.AddPKSApplicationInsights();
        }
    }
}
