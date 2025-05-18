using Microsoft.Extensions.Configuration;
using Peritos.Common.Configuration;

namespace Cpro.Forms.Integration.SendGrid.Configuration;

public interface ISendGridConfig
{
    string APIKey { get; }
    string FromEmail { get; }
    string Name { get; }
    bool IsTesting { get; }
    string TestingEmail { get; }
}

public class SendGridConfig : ConfigurationBase, ISendGridConfig
{
    public SendGridConfig(IConfiguration configuration) : base(configuration, "SendGridConfig")
    {
    }

    public string APIKey { get; set; }
    public string FromEmail { get; set; }
    public string Name { get; set; }
    public bool IsTesting { get; set; }
    public string TestingEmail { get; set; }
}
