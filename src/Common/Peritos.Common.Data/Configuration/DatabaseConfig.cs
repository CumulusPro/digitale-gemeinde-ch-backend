using Microsoft.Extensions.Configuration;
using Peritos.Common.Configuration;

namespace Peritos.Common.Data.Configuration
{
    /// <summary>
    /// Defines the configuration settings required to connect to a database.
    /// </summary>
    public interface IDatabaseConfig
    {
        /// <summary>
        /// Gets the connection string used to connect to the database.
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// Gets the name of the database.
        /// </summary>
        string DatabaseName { get; }
    }

    /// <summary>
    /// Represents the database configuration settings loaded from configuration sources.
    /// </summary>
    public class DatabaseConfig : ConfigurationBase, IDatabaseConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseConfig"/> class.
        /// </summary>
        public DatabaseConfig(IConfiguration configuration) : base(configuration, "DatabaseConfig")
        {
        }

        /// <inheritdoc/>
        public string ConnectionString { get; set; }

        /// <inheritdoc/>
        public string DatabaseName { get; set; }
    }
}
