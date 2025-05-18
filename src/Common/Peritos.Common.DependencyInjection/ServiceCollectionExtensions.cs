using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace Peritos.Common.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds all modules in all assemblies to the service collection. 
        /// </summary>
        /// <param name="serviceCollection">The service collection to use when adding modules. </param>
        /// <returns></returns>
        public static IServiceCollection AddModules(this IServiceCollection serviceCollection)
        {
            var allModules = Peritos.Common.Reflection.AssemblyExtensions.GetSolutionAssemblies()
                                .SelectMany(x => x.GetTypes())
                                .Where(x => x.IsClass && typeof(IServiceCollectionModule).IsAssignableFrom(x))
                                .Select(x => (IServiceCollectionModule)Activator.CreateInstance(x))
                                .ToList();

            allModules.ForEach(x => x.Load(serviceCollection));

            return serviceCollection;
        }

        /// <summary>
        /// Adds all interfaces that match a filter function with their respective implementations. 
        /// </summary>
        /// <param name="services">The current service collection</param>
        /// <param name="filter">The filter used to find the interfaces to bind</param>
        /// <param name="lifetime">The lifetime scope to bind to</param>
        /// <returns></returns>
        public static IServiceCollection AddWithTypeFilter(this IServiceCollection services, Func<Type, bool> filter, ServiceCollectionLifetime lifetime)
        {
            var matchingTypes = Assembly.GetCallingAssembly()
                    .GetTypes()
                    .Where(x => x.IsInterface)
                    .Where(filter);



            foreach(var matchingType in matchingTypes)
            {
                var concreteClasses = Assembly.GetCallingAssembly()
                                        .GetTypes()
                                        .Where(x => x.IsClass && matchingType.IsAssignableFrom(x));

                foreach(var concreteClass in concreteClasses)
                {
                    switch(lifetime)
                    {
                        case ServiceCollectionLifetime.Transient:
                            services.AddTransient(matchingType, concreteClass);
                            break;

                        case ServiceCollectionLifetime.Scoped:
                            services.AddScoped(matchingType, concreteClass);
                            break;

                        case ServiceCollectionLifetime.Singleton:
                            services.AddSingleton(matchingType, concreteClass);
                            break;
                    }
                }
            }

            return services;
        }
    }
}
