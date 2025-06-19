using Microsoft.Extensions.Configuration;
using Peritos.Common.Configuration;

namespace Peritos.Common.Logging.ApplicationInsights
{
    /// <summary>
    /// Configuration interface for Application Insights settings.
    /// </summary>
    public interface IApplicationInsightsConfiguration
    {
        string InstrumentationKey { get; }
        string ApplicationPlatform { get; }
        string ApplicationName { get; }
    }

    /// <summary>
    /// Represents the Application Insights configuration section.
    /// </summary>
    public class ApplicationInsightsConfiguration : ConfigurationBase, IApplicationInsightsConfiguration
    {
        public ApplicationInsightsConfiguration(IConfiguration configuration) : base(configuration, "ApplicationInsights")
        {
        }

        public string InstrumentationKey { get; set; }
        public string ApplicationPlatform { get; set; }
        public string ApplicationName { get; set; }
    }
}
