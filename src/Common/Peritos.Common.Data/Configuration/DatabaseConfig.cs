using Microsoft.Extensions.Configuration;
using Peritos.Common.Configuration;

namespace Peritos.Common.Data.Configuration
{
    public interface IDatabaseConfig
    {
        string ConnectionString { get; }
        string DatabaseName { get; }
    }

    public class DatabaseConfig : ConfigurationBase, IDatabaseConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseConfig"/> class.
        /// </summary>
        public DatabaseConfig(IConfiguration configuration) : base(configuration, "DatabaseConfig")
        {
        }

        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
