using Microsoft.Extensions.DependencyInjection;
using Peritos.Common.DependencyInjection;

namespace Peritos.Common.Mapping
{
    /// <summary>
    /// Service collection module to configure object mapping services.
    /// </summary>
    public class MappingModule : IServiceCollectionModule
    {
        /// <summary>
        /// Registers mapping-related services, including AutoMapper and a common mapper implementation.
        /// </summary>
        /// <param name="services">The service collection to add services to.</param>
        public void Load(IServiceCollection services)
        {
            services.AddTransient<ICommonMapper, CommonMapper>();
            services.AddAutoMapper(Peritos.Common.Reflection.AssemblyExtensions.GetSolutionAssemblies());
        }
    }
}
