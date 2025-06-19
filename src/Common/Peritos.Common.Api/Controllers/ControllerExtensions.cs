using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;
using Peritos.Common.Api.Filters;

namespace Peritos.Common.Api.Controllers
{
    /// <summary>
    /// Provides extension methods for registering and configuring controllers in the service collection.
    /// </summary>
    public static class ControllerExtensions
    {
        /// <summary>
        /// Adds PKS controllers to the service collection and configures global filters, including authentication and JSON serialization.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The application configuration.</param>
        public static void AddPKSControllers(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers(options =>
            {
                //Only apply if authentication is currently set up. 
                if (!string.IsNullOrWhiteSpace(configuration["Authentication:Authority"]))
                {
                    //Add globally required Authentication
                    //Feel free to comment and instead place [Authorize] on the valid logged in endpoints. 
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();

                    options.Filters.Add(new AuthorizeFilter(policy));
                }

                options.Filters.Add(new ProducesAttribute("application/json"));
                options.Filters.Add(typeof(ApiExceptionFilter));
            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });
        }
    }
}
