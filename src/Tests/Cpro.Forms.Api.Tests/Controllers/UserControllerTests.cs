using Cpro.Forms.Api.Controllers;
using Cpro.Forms.Service.Models.Enums;
using Cpro.Forms.Service.Models.User;
using Cpro.Forms.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Peritos.Common.Abstractions.Paging;
using System.Security.Claims;

namespace Cpro.Forms.Api.Tests.Controllers;

public class UserControllerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _controller = new UserController(_userServiceMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task GetUser_ReturnsOkResult_WhenUserExists()
    {
        // Arrange
        var tenantId = 1;
        var email = "user@example.com";
        var expectedUser = new UserResponse { Email = email };

        var claims = new List<Claim>
        {
            new Claim("emails", email)
        };
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));
        var httpContext = new DefaultHttpContext { User = claimsPrincipal };

        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
        _userServiceMock.Setup(x => x.GetUserByEmailAndTenantAsync(email, tenantId))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _controller.GetUser(tenantId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<UserResponse>(okResult.Value);
        Assert.Equal(expectedUser.Email, returnValue.Email);
        _userServiceMock.Verify(x => x.GetUserByEmailAndTenantAsync(email, tenantId), Times.Once);
    }

    [Fact]
    public async Task GetUserById_ReturnsOkResult_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expectedUser = new UserResponse();
        _userServiceMock.Setup(x => x.GetUserByIdAsync(userId))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _controller.GetUserById(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<UserResponse>(okResult.Value);
        Assert.Equal(expectedUser, returnValue);
    }

    [Fact]
    public async Task GetUserById_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userServiceMock.Setup(x => x.GetUserByIdAsync(userId))
            .ReturnsAsync((UserResponse)null);

        // Act
        var result = await _controller.GetUserById(userId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsOkResult_WhenUserCreated()
    {
        // Arrange
        var userRequest = new UserRequest();
        var expectedResponse = new UserResponse();
        _userServiceMock.Setup(x => x.CreateUserAsync(userRequest))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Create(userRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<UserResponse>(okResult.Value);
        Assert.Equal(expectedResponse, returnValue);
    }

    [Fact]
    public async Task Update_ReturnsOkResult_WhenUserUpdated()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userRequest = new UpdateUserRequest();
        var expectedResponse = new UserResponse();
        _userServiceMock.Setup(x => x.UpdateUserAsync(userId, userRequest))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Update(userId, userRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<UserResponse>(okResult.Value);
        Assert.Equal(expectedResponse, returnValue);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenUserDeleted()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var result = await _controller.Delete(userId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _userServiceMock.Verify(x => x.DeleteUserAsync(userId), Times.Once);
    }

    [Fact]
    public async Task AssignRole_ReturnsOkResult_WhenRoleAssigned()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var newRole = Role.Admin;
        var email = "test@example.com";
        var expectedResponse = new UserResponse();
        var claims = new List<Claim>
        {
            new Claim("emails", email)
        };
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));
        var httpContext = new DefaultHttpContext { User = claimsPrincipal };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
        _userServiceMock.Setup(x => x.AssignUserRoleAsync(userId, newRole, email))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.AssignRole(userId, newRole);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<UserResponse>(okResult.Value);
        Assert.Equal(expectedResponse, returnValue);
    }

    [Fact]
    public async Task SearchFormData_ReturnsOkResult_WhenUsersFound()
    {
        // Arrange
        var searchRequest = new UserSearchRequest();
        var expectedResponse = new PagingResponse<UserResponse>();
        _userServiceMock.Setup(x => x.SearchUsers(searchRequest))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.SearchFormData(searchRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<PagingResponse<UserResponse>>(okResult.Value);
        Assert.Equal(expectedResponse, returnValue);
    }

    [Fact]
    public async Task GetTenantsByEmail_ReturnsOkResult_WhenTenantsFound()
    {
        // Arrange
        var email = "user@example.com";
        var expectedTenants = new List<Service.Models.TenantResponse>
        {
            new Service.Models.TenantResponse { Id = 1, Name = "Tenant A" },
            new Service.Models.TenantResponse { Id = 2, Name = "Tenant B" }
        };

        var claims = new List<Claim>
        {
            new Claim("emails", email)
        };
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));
        var httpContext = new DefaultHttpContext { User = claimsPrincipal };

        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
        _userServiceMock.Setup(x => x.GetTenantsByUserEmailAsync(email))
            .ReturnsAsync(expectedTenants);

        // Act
        var result = await _controller.GetTenantsByEmail();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<List<Service.Models.TenantResponse>>(okResult.Value);
        Assert.Equal(expectedTenants.Count, returnValue.Count);
        Assert.Equal(expectedTenants[0].Id, returnValue[0].Id);
        _userServiceMock.Verify(x => x.GetTenantsByUserEmailAsync(email), Times.Once);
    }

}