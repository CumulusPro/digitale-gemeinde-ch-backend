using Microsoft.Extensions.DependencyInjection;
using Peritos.Common.Api.ApplicationInsights;
using Peritos.Common.DependencyInjection;

namespace Peritos.Common.Api.Infrastructure
{
    /// <summary>
    /// Represents a module for configuring API-specific services during application startup.
    /// </summary>
    public class ApiModule : IServiceCollectionModule
    {
        /// <summary>
        /// Registers API-related services into the dependency injection container.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to which services are added.</param>
        public void Load(IServiceCollection services)
        {
            services.AddPKSApplicationInsights();
        }
    }
}
