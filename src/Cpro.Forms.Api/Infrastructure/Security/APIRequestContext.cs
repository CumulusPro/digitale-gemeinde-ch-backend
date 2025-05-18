using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Peritos.Common.Abstractions;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Cpro.Forms.Api.Infrastructure.Security
{
    public class ApiRequestContext : IRequestContext
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ApiRequestContext> _logger;

        //Normally we would never inject ServiceProvider, but this needs to run with the absolute minimal dependency tree. 
        public ApiRequestContext(IServiceProvider serviceProvider, ILogger<ApiRequestContext> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        //READ ME
        //The first thing you need to check is which claim holds the users email/identifier. Then change that below (It is currently correct for B2C)
        //Then go to the commented out section, and write code that calls your repository/service and gets a user by that email
        //Then you can set the UserId/RoleId for the current claimed user. 
        //From there, you can now inject IRequestContext ANYWHERE (including in services) to check which user and which role they currently have. 
        //Pretty nice!
        public async Task LoadApiRequestContext()
        {
            var httpContextAccessor = _serviceProvider.GetRequiredService<IHttpContextAccessor>();

            if (!httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                return;
            }

            var headers = httpContextAccessor.HttpContext.Request.Headers;
            if (headers.ContainsKey("Authorization"))
            {
                var authHeader = headers["Authorization"].ToString();
                Console.WriteLine($"Authorization Header: {authHeader}");
                _logger.LogInformation($"Authorization Header: {authHeader}");

                // If you want just the token (e.g., "Bearer abc.def.ghi")
                if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    Token = authHeader;
                    var token = authHeader.Substring("Bearer ".Length).Trim();
                    Console.WriteLine($"Token: {token}");
                    _logger.LogInformation($"Token: {token}");
                }
            }

            foreach (var claim in httpContextAccessor.HttpContext.User.Claims)
            {
                Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
                _logger.LogInformation($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
            }

            IsAuthenticated = true;
            if (httpContextAccessor.HttpContext.User.Claims.Any(c => c.Type == "emails"))
            {
                UserEmail = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "emails")?.Value;
            }
            else 
            {
                UserEmail = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                Console.WriteLine($"Claim Type: {ClaimTypes.Email}, Claim Value: {UserEmail}");
            }
            await Task.CompletedTask;
        }

        public bool IsAuthenticated { get; set; }
        public string UserEmail { get; set; }
        public int? UserId { get; set; }
        public int? RoleId { get; set; }
        public string? Token { get; set; }
    }
}
