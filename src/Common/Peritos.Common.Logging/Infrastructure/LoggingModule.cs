using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using Peritos.Common.DependencyInjection;
using Peritos.Common.Logging.ApplicationInsights;

namespace Peritos.Common.Logging.Infrastructure
{
    /// <summary>
    /// Service collection module to configure logging and Application Insights telemetry.
    /// </summary>
    public class LoggingModule : IServiceCollectionModule
    {
        /// <summary>
        /// Registers services related to logging and telemetry, including the custom telemetry initializer.
        /// </summary>
        /// <param name="services">The service collection to add services to.</param>
        public void Load(IServiceCollection services)
        {
            services.AddSingleton<ITelemetryInitializer, CustomPropertyTelemetryInitializer>();
            services.AddApplicationInsightsTelemetry();
        }
    }
}
