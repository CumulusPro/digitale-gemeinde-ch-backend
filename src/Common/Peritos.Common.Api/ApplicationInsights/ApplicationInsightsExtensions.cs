using Microsoft.Extensions.DependencyInjection;
using Peritos.Common.Logging.Abstractions;

namespace Peritos.Common.Api.ApplicationInsights
{
    /// <summary>
    /// Provides extension methods for registering Application Insights services.
    /// </summary>
    public static class ApplicationInsightsExtensions
    {
        /// <summary>
        /// Adds Application Insights telemetry and correlation service to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public static void AddPKSApplicationInsights(this IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();
            services.AddTransient<ICorrelationService, CorrelationService>();
        }
    }
}
