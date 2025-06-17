using Cpro.Forms.Api.Infrastructure.Security;
using Peritos.Common.Abstractions;

namespace Cpro.Forms.Api.Infrastructure.Middleware
{
    public class ApiRequestContextMiddleware
	{
		private readonly RequestDelegate _next;

		public ApiRequestContextMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext httpContext, IRequestContext requestContext)
		{
			if (!(requestContext is ApiRequestContext))
			{
				throw new ArgumentException("Expecting IRequestContext to be of type APIRequestContext");
			}

			var apiRequestContext = requestContext as ApiRequestContext;
			await apiRequestContext.LoadApiRequestContext();

			await _next(httpContext);
		}
	}

	public static class ApiRequestContextMiddlewareExtensions
	{
		public static IApplicationBuilder UseApiRequestContext(
			this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<ApiRequestContextMiddleware>();
		}
	}
}
