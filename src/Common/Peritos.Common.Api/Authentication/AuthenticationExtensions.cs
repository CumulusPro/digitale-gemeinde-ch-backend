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
    public static class AuthenticationExtensions
    {
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

        public static void UsePKSAuthentication(this IApplicationBuilder app, IConfiguration configuration)
        {
            if (!string.IsNullOrWhiteSpace(configuration["Authentication:Authority"]))
            {
                app.UseAuthentication();
                app.UseAuthorization();
            }
        }
    }


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

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Return NoResult so that authentication is essentially skipped.
            return Task.FromResult(AuthenticateResult.NoResult());
        }
    }
}
