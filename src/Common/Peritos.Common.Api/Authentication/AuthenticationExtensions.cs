using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;


namespace Peritos.Common.Api.Authentication
{
    /// <summary>
    /// Provides extension methods for configuring authentication in the application.
    /// </summary>
    public static class AuthenticationExtensions
    {
        /// <summary>
        /// Adds PKS authentication schemes (B2C, AAD, None) to the service collection based on configuration.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The application configuration.</param>
        public static void AddPKSAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            if (!string.IsNullOrWhiteSpace(configuration["Authentication:Authority"]))
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "B2C_OR_AAD";
                    options.DefaultChallengeScheme = "B2C_OR_AAD";
                })
                .AddScheme<AuthenticationSchemeOptions, NoAuthenticationHandler>("None", options => { })
                .AddJwtBearer("B2C", jwtOptions =>
                {
                    jwtOptions.Authority = configuration["Authentication:Authority"];
                    jwtOptions.Audience = configuration["Authentication:Audience"];
                })
                .AddJwtBearer("AAD", jwtOptions =>
                {
                    jwtOptions.Authority = configuration["Authentication:AADAuthority"];
                    jwtOptions.Audience = configuration["Authentication:Audience"];
                })
                .AddPolicyScheme("B2C_OR_AAD", "B2C_OR_AAD", options =>
                {
                    options.ForwardDefaultSelector = context =>
                    {
                        string authorization = context.Request.Headers[HeaderNames.Authorization];
                        if (!string.IsNullOrWhiteSpace(authorization) && authorization.StartsWith("Bearer "))
                        {
                            var token = authorization.Substring("Bearer ".Length).Trim();
                            var jwtHandler = new JwtSecurityTokenHandler();
                            var tfp = jwtHandler.ReadJwtToken(token).Claims.FirstOrDefault(x=>x.Type == "tfp")?.Value;
                            if (string.IsNullOrWhiteSpace(tfp))
                                return "None";
                            return (jwtHandler.CanReadToken(token) && tfp.Equals("B2C_1A_signup_signin"))
                                ? "B2C" : "AAD";
                        }
                        return "AAD";
                    };
                });

            }
        }

        /// <summary>
        /// Configures the application to use authentication and authorization middleware if authentication is set up.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="configuration">The application configuration.</param>
        public static void UsePKSAuthentication(this IApplicationBuilder app, IConfiguration configuration)
        {
            if (!string.IsNullOrWhiteSpace(configuration["Authentication:Authority"]))
            {
                app.UseAuthentication();
                app.UseAuthorization();
            }
        }
    }

    /// <summary>
    /// Authentication handler that always returns no result, effectively disabling authentication.
    /// </summary>
    public class NoAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public NoAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        /// <summary>
        /// Handles authentication by returning no result.
        /// </summary>
        /// <returns>An authentication result indicating no result.</returns>
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Return NoResult so that authentication is essentially skipped.
            return Task.FromResult(AuthenticateResult.NoResult());
        }
    }
}
