using Microsoft.AspNetCore.Http;
using Peritos.Common.Logging.Abstractions;

namespace Peritos.Common.Api.ApplicationInsights
{
    /// <summary>
    /// Provides correlation ID functionality for tracking requests across services.
    /// </summary>
    public class CorrelationService : ICorrelationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CorrelationService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Gets the correlation ID from the current HTTP request headers.
        /// </summary>
        public string CorrelationId
        {
            get
            {
                var context = _httpContextAccessor?.HttpContext;
                if (context == null)
                {
                    return string.Empty;
                }

                return context.Request.Headers["x-correlation-id"];
            }
        }

    }
}
