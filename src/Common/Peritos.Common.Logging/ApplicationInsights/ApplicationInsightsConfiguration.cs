using Microsoft.Extensions.Configuration;
using Peritos.Common.Configuration;

namespace Peritos.Common.Logging.ApplicationInsights
{
    public interface IApplicationInsightsConfiguration
    {
        string InstrumentationKey { get; }
        string ApplicationPlatform { get; }
        string ApplicationName { get; }
    }

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
