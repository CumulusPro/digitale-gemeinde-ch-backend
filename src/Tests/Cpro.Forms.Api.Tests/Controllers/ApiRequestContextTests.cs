using Cpro.Forms.Api.Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace Cpro.Forms.Api.Tests.Controllers;

public class ApiRequestContextTests
{
    private DefaultHttpContext CreateHttpContextWithClaimsAndAuthHeader(
        string emailClaimType,
        string email,
        string authHeader = "Bearer abc.def.ghi")
    {
        var claims = new List<Claim>
        {
            new Claim(emailClaimType, email)
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        var context = new DefaultHttpContext { User = user };
        context.Request.Headers["Authorization"] = authHeader;

        return context;
    }

    [Fact]
    public async Task LoadApiRequestContext_SetsEmailAndToken_WhenEmailsClaimPresent()
    {
        // Arrange
        var email = "user1@example.com";
        var context = CreateHttpContextWithClaimsAndAuthHeader("emails", email);
        var accessor = new Mock<IHttpContextAccessor>();
        accessor.Setup(x => x.HttpContext).Returns(context);

        var sp = new Mock<IServiceProvider>();
        sp.Setup(x => x.GetService(typeof(IHttpContextAccessor))).Returns(accessor.Object);

        var logger = new Mock<ILogger<ApiRequestContext>>();
        var requestContext = new ApiRequestContext(sp.Object, logger.Object);

        // Act
        await requestContext.LoadApiRequestContext();

        // Assert
        Assert.True(requestContext.IsAuthenticated);
        Assert.Equal(email, requestContext.UserEmail);
        Assert.Equal("Bearer abc.def.ghi", requestContext.Token);
    }

    [Fact]
    public async Task LoadApiRequestContext_FallsBackToClaimTypesEmail_WhenEmailsClaimMissing()
    {
        // Arrange
        var email = "fallback@example.com";
        var context = CreateHttpContextWithClaimsAndAuthHeader(ClaimTypes.Email, email);
        var accessor = new Mock<IHttpContextAccessor>();
        accessor.Setup(x => x.HttpContext).Returns(context);

        var sp = new Mock<IServiceProvider>();
        sp.Setup(x => x.GetService(typeof(IHttpContextAccessor))).Returns(accessor.Object);

        var logger = new Mock<ILogger<ApiRequestContext>>();
        var requestContext = new ApiRequestContext(sp.Object, logger.Object);

        // Act
        await requestContext.LoadApiRequestContext();

        // Assert
        Assert.True(requestContext.IsAuthenticated);
        Assert.Equal(email, requestContext.UserEmail);
        Assert.Equal("Bearer abc.def.ghi", requestContext.Token);
    }

    [Fact]
    public async Task LoadApiRequestContext_DoesNothing_WhenUserIsNotAuthenticated()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity()); // Not authenticated

        var accessor = new Mock<IHttpContextAccessor>();
        accessor.Setup(x => x.HttpContext).Returns(context);

        var sp = new Mock<IServiceProvider>();
        sp.Setup(x => x.GetService(typeof(IHttpContextAccessor))).Returns(accessor.Object);

        var logger = new Mock<ILogger<ApiRequestContext>>();
        var requestContext = new ApiRequestContext(sp.Object, logger.Object);

        // Act
        await requestContext.LoadApiRequestContext();

        // Assert
        Assert.False(requestContext.IsAuthenticated);
        Assert.Null(requestContext.UserEmail);
        Assert.Null(requestContext.Token);
    }
}
