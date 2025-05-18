using Microsoft.Extensions.DependencyInjection;

namespace Peritos.Common.DependencyInjection
{
    public interface IServiceCollectionModule
    {
        /// <summary>
        /// Called when the executing assembly wants to load all modules. 
        /// </summary>
        /// <param name="services">The current service collection</param>
        void Load(IServiceCollection services);
    }
}
