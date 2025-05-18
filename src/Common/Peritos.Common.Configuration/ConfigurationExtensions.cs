using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;

namespace Peritos.Common.Configuration
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Adds KeyVault configuration similar following Microsoft Documentation (https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-3.1)
        /// </summary>
        /// <param name="configurationBuilder"></param>
        public static void ConfigureAppConfigurationKeyVault(HostBuilderContext hostBuilderContext, IConfigurationBuilder configurationBuilder)
        {
            var builtConfig = configurationBuilder.Build();

            // In production, enforce keyvault usage. 
             if (string.IsNullOrWhiteSpace(builtConfig["KeyVault:Vault"]))
            {
                //If we aren't in Development, enforce keyvault usage. 
                // if (!hostBuilderContext.HostingEnvironment.IsDevelopment() && !hostBuilderContext.HostingEnvironment.IsEnvironment("Local"))
                // {
                //     throw new NullReferenceException("Unable to read KeyVault secrets as the config setting KeyVault:Vault does not contain a vault ID");
                // } else
                {
                    return;
                }
            }
            //Remove all options except CLI (For Dev), and Managed Identity (For Azure VM/Functions/App Servce etc)
            var credentialOptions = new DefaultAzureCredentialOptions
            {
                ExcludeEnvironmentCredential = true,
                ExcludeVisualStudioCredential = true,
                ExcludeVisualStudioCodeCredential = true,
                ExcludeInteractiveBrowserCredential = true,
                ExcludeSharedTokenCacheCredential = true
            };

            var keyVaultClient = new SecretClient(new Uri($"https://{builtConfig["KeyVault:Vault"]}.vault.azure.net/"),
                new DefaultAzureCredential(credentialOptions));

            configurationBuilder.AddAzureKeyVault(keyVaultClient, new KeyVaultSecretManager());
        }
    }
}
