using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Peritos.Common.Api.Cors
{
    public static class CorsExtensions
    {
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
