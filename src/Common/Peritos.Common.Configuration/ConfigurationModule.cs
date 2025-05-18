using Microsoft.Extensions.DependencyInjection;
using Peritos.Common.DependencyInjection;
using System;
using System.Linq;

namespace Peritos.Common.Configuration
{
    public class ConfigurationModule : IServiceCollectionModule
    {
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
