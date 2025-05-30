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

            // See the src code for the DefaultProblemDetailsFactory here - https://git.io/JelA6
            Func<int, string, ProblemDetails> ExceptionBody = (statusCode, messageBody) =>
            {
                return _problemDetailsFactory.CreateProblemDetails(
                    httpContext: context.HttpContext,
                    statusCode: statusCode,
                    title: messageBody,
                    type: null);            // factory populates this with sensible default based on the status code
            };

            switch (context.Exception)
            {
                case ApiException apiEx:
                    _logger.LogError(apiEx, "ApiException");
                    context.Result = new JsonResult(ExceptionBody(apiEx.HttpStatusCode, apiEx.Message))
                    {
                        StatusCode = apiEx.HttpStatusCode
                    };
                    context.ExceptionHandled = true;
                    break;

                case UnauthorizedAccessException authEx:
                    _logger.LogError(authEx, "Unauthorized Access Exception");
                    context.Result = new JsonResult(ExceptionBody(403, "user does not have permission to access this resource"))
                    {
                        StatusCode = 403
                    };
                    context.ExceptionHandled = true;
                    break;

                case KeyNotFoundException keyNotFoundEx:
                    _logger.LogError(keyNotFoundEx, "KeyNotFoundException: {Message}", keyNotFoundEx.Message);
                    context.Result = new JsonResult(ExceptionBody(404, "The requested resource was not found"))
                    {
                        StatusCode = 404
                    };
                    context.ExceptionHandled = true;
                    break;

                case InvalidOperationException invalidOpEx:
                    _logger.LogError(invalidOpEx, "InvalidOperationException: {Message}", invalidOpEx.Message);
                    context.Result = new JsonResult(ExceptionBody(400, "Invalid operation: "))
                    {
                        StatusCode = 400
                    };
                    context.ExceptionHandled = true;
                    break;

                case HttpRequestException httpRequestException:
                    _logger.LogError(httpRequestException, "HttpRequestException: {Message}", httpRequestException.Message);
                    context.Result = new JsonResult(ExceptionBody(400, "Invalid operation: "))
                    {
                        StatusCode = 400
                    };
                    context.ExceptionHandled = true;
                    break;

                default:
                    _logger.LogError(context.Exception, "Exception");
                    context.Result = new JsonResult(ExceptionBody(500, context.Exception.Message))
                    {
                        StatusCode = 500
                    };
                    break;
            }
        }
    }
}
