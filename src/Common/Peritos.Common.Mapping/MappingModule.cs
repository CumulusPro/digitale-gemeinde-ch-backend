using Microsoft.Extensions.DependencyInjection;
using Peritos.Common.DependencyInjection;

namespace Peritos.Common.Mapping
{
    public class MappingModule : IServiceCollectionModule
    {
        public void Load(IServiceCollection services)
        {
            services.AddTransient<ICommonMapper, CommonMapper>();
            services.AddAutoMapper(Peritos.Common.Reflection.AssemblyExtensions.GetSolutionAssemblies());
        }
    }
}
