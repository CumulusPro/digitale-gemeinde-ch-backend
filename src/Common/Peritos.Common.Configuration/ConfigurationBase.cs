using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;

namespace Peritos.Common.Configuration
{
    /// <summary>
    /// Abstract Configuration Class that will get the settings for the current system
    /// </summary>
    public abstract class ConfigurationBase
    {
        /// <summary>
        /// Name of the system configuration we are working with
        /// </summary>
        private readonly string _keyPrefix;

        /// <summary>
        /// Configuration Builder we are targeting
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationBase"/> class.
        /// </summary>
        /// <param name="keyPrefix">Name of the system configuration we are working with</param>
        protected ConfigurationBase(IConfiguration configuration, string keyPrefix)
        {
            _configuration = configuration;
            _keyPrefix = keyPrefix;
            _configuration.GetSection(keyPrefix).Bind(this);
        }

        /// <summary>
        /// Gets the setting.
        /// </summary>
        /// <param name="settingName">Name of the setting.</param>
        /// <param name="defaultValue">The default value if no setting is found.</param>
        /// <param name="ignorePrefix">if set to <c>true</c> [ignore prefix].</param>
        /// <returns>the requested setting.</returns>
        protected string GetSetting(string settingName, string defaultValue, bool ignorePrefix = false)
        {
            string key = ignorePrefix
                    ? settingName
                    : $"{_keyPrefix}:{settingName}";
            return Environment.GetEnvironmentVariable($"APPSETTING_{key}") ??
                    _configuration.GetSection(key).Value ?? defaultValue;
        }

        /// <summary>
        /// Gets the setting as boolean.
        /// </summary>
        /// <param name="settingName">Name of the setting.</param>
        /// <param name="defaultValue">The default value if no setting is found.</param>
        /// <param name="ignorePrefix">if set to <c>true</c> [ignore prefix].</param>
        /// <returns>the requested setting.</returns>
        protected bool GetSettingAsBoolean(string settingName, bool defaultValue, bool ignorePrefix = false)
        {
            string rawValue = this.GetSetting(settingName, defaultValue.ToString(), ignorePrefix);
            bool value = bool.TrueString.Equals(rawValue, StringComparison.OrdinalIgnoreCase);
            return value;
        }

        /// <summary>
        /// Gets the setting as integer.
        /// </summary>
        /// <param name="settingName">Name of the setting.</param>
        /// <param name="defaultValue">The default value if no setting is found.</param>
        /// <param name="ignorePrefix">if set to <c>true</c> [ignore prefix].</param>
        /// <returns>the requested setting.</returns>
        protected int GetSettingAsInteger(string settingName, int defaultValue, bool ignorePrefix = false)
        {
            string rawValue = this.GetSetting(settingName, defaultValue.ToString(), ignorePrefix);
            bool wasParsed = int.TryParse(rawValue, out var parsedValue);
            return wasParsed
                    ? parsedValue
                    : defaultValue;
        }

        /// <summary>
        /// Gets the setting as double.
        /// </summary>
        /// <param name="settingName">Name of the setting.</param>
        /// <param name="defaultValue">The default value if no setting is found.</param>
        /// <param name="ignorePrefix">if set to <c>true</c> [ignore prefix].</param>
        /// <returns>the requested setting.</returns>
        protected double GetSettingAsDouble(string settingName, double defaultValue, bool ignorePrefix = false)
        {
            string rawValue = this.GetSetting(
                    settingName,
                    defaultValue.ToString(CultureInfo.InvariantCulture),
                    ignorePrefix);
            bool wasParsed = double.TryParse(rawValue, out var parsedValue);
            return wasParsed
                    ? parsedValue
                    : defaultValue;
        }

        /// <summary>
        /// Returns a source setting as a byte array
        /// </summary>
        /// <param name="settingName">Setting Name we a retrieving</param>
        /// <param name="defaultValue">Setting Default Value</param>
        /// <returns>The Setting as a byte array</returns>
        protected byte[] GetSettingAsByteArray(string settingName, string defaultValue)
        {
            string rawValue = this.GetSetting(settingName, defaultValue);
            byte[] parsedValue = Convert.FromBase64String(rawValue);
            return parsedValue;
        }

        /// <summary>
        /// Gets a setting from the configuration source and parses it to a <see cref="Uri"/>.
        /// </summary>
        /// <param name="settingName">The setting name for the setting to be retrieved.</param>
        /// <returns>A <see cref="Uri"/> initialized from the value the configuration store returns.</returns>
        protected Uri GetSettingAsUri(string settingName)
        {
            string settingValue = this.GetSettingAsString(settingName);
            return new Uri(settingValue);
        }

        /// <summary>
        /// Gets a setting string from the configuration source.
        /// </summary>
        /// <param name="settingName">The setting name for the setting to be retrieved.</param>
        /// <returns>The setting string.</returns>
        protected string GetSettingAsString(string settingName)
        {
            string settingValue = this.GetSetting(settingName, string.Empty);
            if (string.IsNullOrWhiteSpace(settingValue))
            {
                throw new ArgumentNullException($"The setting {_keyPrefix} {settingName} is null or empty.");
            }

            return settingValue;
        }
    }
}
