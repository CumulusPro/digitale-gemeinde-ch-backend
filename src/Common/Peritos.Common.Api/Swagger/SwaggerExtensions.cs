using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;

namespace Peritos.Common.Api.Swagger
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddPKSSwagger(this IServiceCollection services, IConfiguration configuration, 
                                                    Action<SwaggerGenOptions> additionalSwaggerOptions = null)
        {
            services.AddSwaggerGenNewtonsoftSupport();

            return services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                        new OpenApiInfo
                        {
                            Title = "API",
                            Version = "v1",
                            Description = "A REST API"
                        });

                c.OperationFilter<SwaggerJsonIgnore>();
                c.CustomSchemaIds(type => type.ToString());

                //Adds swagger authentication if the correct configuration is set up. 
                if (!string.IsNullOrWhiteSpace(configuration["Authentication:Swagger:Scope"]))
                {
                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows
                        {
                            Implicit = new OpenApiOAuthFlow
                            {
                                Scopes = new Dictionary<string, string>
                                {
                                    { configuration["Authentication:Swagger:Scope"], "Access the api as the signed-in user" }
                                },
                                AuthorizationUrl = new Uri(configuration["Authentication:Swagger:AuthorizationUrl"]),
                                TokenUrl = new Uri(configuration["Authentication:Swagger:AuthorizationUrl"])
                            }
                        }
                    });

                    c.OperationFilter<SwaggerSecurityRequirementsOperationFilter>();
                }

                additionalSwaggerOptions?.Invoke(c);
            });
        }

        public static void UsePKSSwagger(this IApplicationBuilder app, IConfiguration configuration, Action<SwaggerUIOptions> additionalSwaggerOptions = null)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API");
                if (!string.IsNullOrWhiteSpace(configuration["Authentication:Swagger:Scope"]))
                {
                    c.OAuthClientId(configuration["Authentication:Audience"]);
                }
                additionalSwaggerOptions?.Invoke(c);
            });
        }
    }
}
