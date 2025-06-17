using Cpro.Forms.Api.Infrastructure.Middleware;
using Microsoft.AspNetCore.Http;
using Moq;
using Peritos.Common.Abstractions;

namespace Cpro.Forms.Api.Tests;

public class ApiRequestContextMiddlewareTests
{
    [Fact]
    public async Task Invoke_ThrowsArgumentException_WhenRequestContextIsInvalid()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();

        var next = new RequestDelegate(ctx => Task.CompletedTask);

        var invalidRequestContext = new Mock<IRequestContext>(); // Not ApiRequestContext
        var middleware = new ApiRequestContextMiddleware(next);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => middleware.Invoke(httpContext, invalidRequestContext.Object));

        Assert.Equal("Expecting IRequestContext to be of type APIRequestContext", exception.Message);
    }
}
