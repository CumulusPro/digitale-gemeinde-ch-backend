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
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger, ProblemDetailsFactory problemDetailsFactory)
        {
            _logger = logger;
            _problemDetailsFactory = problemDetailsFactory;
        }

        private readonly ILogger _logger;
        private readonly ProblemDetailsFactory _problemDetailsFactory;

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
