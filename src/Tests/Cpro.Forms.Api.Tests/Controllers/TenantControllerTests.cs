using Cpro.Forms.Api.Controllers;
using Cpro.Forms.Service.Interface;
using Cpro.Forms.Service.Models.Tenant;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Peritos.Common.Abstractions.Paging;

namespace Cpro.Forms.Api.Tests.Controllers;

public class TenantControllerTests
{
    private readonly Mock<ITenantService> _tenantServiceMock;
    private readonly TenantController _controller;

    public TenantControllerTests()
    {
        _tenantServiceMock = new Mock<ITenantService>();
        _controller = new TenantController(_tenantServiceMock.Object);
    }

    [Fact]
    public async Task Get_ReturnsOkResult_WhenTenantExists()
    {
        // Arrange
        var tenantId = 1;
        var expectedTenant = new TenantResponse();
        _tenantServiceMock.Setup(x => x.GetTenantByIdAsync(tenantId))
            .ReturnsAsync(expectedTenant);

        // Act
        var result = await _controller.Get(tenantId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<TenantResponse>(okResult.Value);
        Assert.Equal(expectedTenant, returnValue);
    }

    [Fact]
    public async Task Get_ReturnsNotFound_WhenTenantDoesNotExist()
    {
        // Arrange
        var tenantId = 1;
        _tenantServiceMock.Setup(x => x.GetTenantByIdAsync(tenantId))
            .ReturnsAsync((TenantResponse)null);

        // Act
        var result = await _controller.Get(tenantId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsOkResult_WhenTenantCreated()
    {
        // Arrange
        var tenantRequest = new TenantRequest();
        var expectedResponse = new TenantResponse();
        _tenantServiceMock.Setup(x => x.CreateTenantAsync(tenantRequest))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Create(tenantRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<TenantResponse>(okResult.Value);
        Assert.Equal(expectedResponse, returnValue);
    }

    [Fact]
    public async Task Update_ReturnsOkResult_WhenTenantUpdated()
    {
        // Arrange
        var tenantId = 1;
        var tenantRequest = new TenantRequest();
        var expectedResponse = new TenantResponse();
        _tenantServiceMock.Setup(x => x.UpdateTenantAsync(tenantId, tenantRequest))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Update(tenantId, tenantRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<TenantResponse>(okResult.Value);
        Assert.Equal(expectedResponse, returnValue);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenTenantDeleted()
    {
        // Arrange
        var tenantId = 1;

        // Act
        var result = await _controller.Delete(tenantId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _tenantServiceMock.Verify(x => x.DeleteTenantAsync(tenantId), Times.Once);
    }

    [Fact]
    public async Task SearchFormData_ReturnsOkResult_WhenTenantsFound()
    {
        // Arrange
        var searchRequest = new TenantSearchRequest();
        var expectedResponse = new PagingResponse<TenantResponse>();
        _tenantServiceMock.Setup(x => x.SearchTenants(searchRequest))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.SearchTenants(searchRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<PagingResponse<TenantResponse>>(okResult.Value);
        Assert.Equal(expectedResponse, returnValue);
    }
} 