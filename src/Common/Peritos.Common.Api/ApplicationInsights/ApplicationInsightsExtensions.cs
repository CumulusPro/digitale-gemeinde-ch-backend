using Microsoft.Extensions.DependencyInjection;
using Peritos.Common.Logging.Abstractions;

namespace Peritos.Common.Api.ApplicationInsights
{
    public static class ApplicationInsightsExtensions
    {
        public static void AddPKSApplicationInsights(this IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();
            services.AddTransient<ICorrelationService, CorrelationService>();
        }
    }
}
