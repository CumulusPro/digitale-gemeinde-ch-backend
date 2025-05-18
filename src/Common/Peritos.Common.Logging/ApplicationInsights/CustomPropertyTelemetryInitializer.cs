using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using Peritos.Common.Logging.Abstractions;
using System;
using System.Reflection;

namespace Peritos.Common.Logging.ApplicationInsights
{
    public class CustomPropertyTelemetryInitializer : ITelemetryInitializer
    {
        private readonly IServiceProvider _serviceProvider;

        public CustomPropertyTelemetryInitializer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Initialize(ITelemetry telemetry)
        {
            var correlationService = _serviceProvider.GetRequiredService<ICorrelationService>();
            var appInsightsConfiguration = _serviceProvider.GetRequiredService<IApplicationInsightsConfiguration>();

            ((ISupportProperties)telemetry).Properties["CorrelationId"] = correlationService.CorrelationId;

            telemetry.Context.GlobalProperties["ApplicationPlatform"] =  appInsightsConfiguration.ApplicationPlatform;
            telemetry.Context.GlobalProperties["ApplicationVersion"] = Assembly.GetEntryAssembly().GetName().Version.ToString();
            telemetry.Context.GlobalProperties["ApplicationName"] = appInsightsConfiguration.ApplicationName;
            telemetry.Context.GlobalProperties["HostId"] =  Environment.MachineName;
        }
    }
}
