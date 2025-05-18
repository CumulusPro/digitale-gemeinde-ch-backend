using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using Peritos.Common.DependencyInjection;
using Peritos.Common.Logging.ApplicationInsights;

namespace Peritos.Common.Logging.Infrastructure
{
    public class LoggingModule : IServiceCollectionModule
    {
        public void Load(IServiceCollection services)
        {
            services.AddSingleton<ITelemetryInitializer, CustomPropertyTelemetryInitializer>();
            services.AddApplicationInsightsTelemetry();
        }
    }
}
