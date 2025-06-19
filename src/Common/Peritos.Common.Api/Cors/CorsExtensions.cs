using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Peritos.Common.Api.Cors
{
    /// <summary>
    /// Provides extension methods for configuring CORS in the application.
    /// </summary>
    public static class CorsExtensions
    {
        /// <summary>
        /// Configures and enables CORS based on the provided configuration.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="configuration">The application configuration.</param>
        public static void UsePKSCors(this IApplicationBuilder app, IConfiguration configuration)
        {
            if (!string.IsNullOrWhiteSpace(configuration["CorsConfiguration:Origin"]))
            {
                app.UseCors(x =>
                {
                    if (configuration["CorsConfiguration:Origin"] == "*")
                    {
                        x.WithMethods("OPTIONS", "POST", "GET", "PUT")
                        .AllowAnyHeader()
                        .WithOrigins("http://localhost:4200");//.AllowCredentials();
                    }
                    else
                    {
                        x.WithOrigins(configuration["CorsConfiguration:Origin"]).AllowAnyHeader().AllowAnyMethod();
                    }
                });
            }
        }
    }
}
