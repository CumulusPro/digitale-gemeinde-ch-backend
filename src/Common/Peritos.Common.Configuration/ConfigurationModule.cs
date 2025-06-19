using Microsoft.Extensions.DependencyInjection;
using Peritos.Common.DependencyInjection;
using System;
using System.Linq;

namespace Peritos.Common.Configuration
{
    /// <summary>
    /// Module responsible for registering configuration classes that inherit from <see cref="ConfigurationBase"/> 
    /// into the dependency injection container as singletons.
    /// </summary>
    public class ConfigurationModule : IServiceCollectionModule
    {
        /// <summary>
        /// Scans all assemblies in the solution for non-abstract classes that inherit from <see cref="ConfigurationBase"/>,
        /// and registers them as singleton services using their first implemented interface.
        /// Throws an <see cref="InvalidOperationException"/> if any configuration class does not implement an interface.
        /// </summary>
        /// <param name="services">The service collection to which configuration services will be added.</param>
        public void Load(IServiceCollection services)
        {
            var configurations = Peritos.Common.Reflection.AssemblyExtensions.GetSolutionAssemblies()
                                .SelectMany(x => x.GetTypes())
                                .Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(typeof(ConfigurationBase)));

            foreach(var configuration in configurations)
            {
                var configurationInterface = configuration.GetInterfaces().FirstOrDefault();
                if(configurationInterface == null)
                {
                    throw new InvalidOperationException($"All configuration objects must use interfaces. {configuration.FullName} does not.");
                }
                services.AddSingleton(configurationInterface, configuration);
            }
        }
    }
}
