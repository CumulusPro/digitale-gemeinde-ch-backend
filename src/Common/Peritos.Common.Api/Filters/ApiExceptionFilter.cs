using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Peritos.Common.Abstractions;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Peritos.Common.Api.Filters
{
    /// <summary>
    /// Exception filter that handles API exceptions and maps them to appropriate HTTP responses.
    /// </summary>
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger _logger;
        private readonly ProblemDetailsFactory _problemDetailsFactory;

        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger, ProblemDetailsFactory problemDetailsFactory)
        {
            _logger = logger;
            _problemDetailsFactory = problemDetailsFactory;
        }

        /// <summary>
        /// Called when an exception occurs. Maps exceptions to HTTP responses and logs the error.
        /// </summary>
        /// <param name="context">The exception context.</param>
        public override void OnException(ExceptionContext context)
        {
            if (context.ExceptionHandled)
                return;

            var exception = context.Exception;

            var (statusCode, message) = MapExceptionToResponse(exception);

            _logger.LogError(exception, $"{exception.GetType().Name}: {message}");

            var problemDetails = _problemDetailsFactory.CreateProblemDetails(
                context.HttpContext,
                statusCode: statusCode,
                title: message
            );

            context.Result = new JsonResult(problemDetails)
            {
                StatusCode = statusCode
            };

            context.ExceptionHandled = true;
        }

        /// <summary>
        /// Maps an exception to an HTTP status code and message.
        /// </summary>
        /// <param name="exception">The exception to map.</param>
        /// <returns>A tuple containing the status code and message.</returns>
        private (int statusCode, string message) MapExceptionToResponse(Exception exception)
        {
            return exception switch
            {
                ApiException apiEx => (apiEx.HttpStatusCode, apiEx.Message),
                UnauthorizedAccessException => (403, "User does not have permission to access this resource"),
                KeyNotFoundException => (404, "The requested resource was not found"),
                InvalidOperationException => (400, "Invalid operation"),
                HttpRequestException => (400, "Invalid request"),
                _ => (500, "An unexpected error occurred")
            };
        }
    }
}
