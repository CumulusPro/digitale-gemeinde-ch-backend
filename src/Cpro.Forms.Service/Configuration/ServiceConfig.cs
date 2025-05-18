using Microsoft.Extensions.Configuration;
using Peritos.Common.Configuration;

namespace Cpro.Forms.Service.Configuration;

public interface IServiceConfig
{
    bool UseStraatos { get; }
}

public class ServiceConfig : ConfigurationBase, IServiceConfig
{
    public ServiceConfig(IConfiguration configuration) : base(configuration, "ServiceConfiguration")
    {
    }

    public bool UseStraatos { get; set; }
}
