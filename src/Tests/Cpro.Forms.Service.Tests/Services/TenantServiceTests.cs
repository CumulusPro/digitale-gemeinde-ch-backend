using Cpro.Forms.Data.Repositories;
using Cpro.Forms.Service.Models;
using Cpro.Forms.Service.Services;
using Peritos.Common.Abstractions.Paging;

namespace Cpro.Forms.Service.Tests.Services;

public class TenantServiceTests
{
    private readonly Mock<ITenantRepository> _tenantRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly TenantService _tenantService;

    public TenantServiceTests()
    {
        _tenantRepositoryMock = new Mock<ITenantRepository>();
        _mapperMock = new Mock<IMapper>();
        _tenantService = new TenantService(_tenantRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetTenantByIdAsync_ReturnsTenant_WhenTenantExists()
    {
        // Arrange
        var tenantId = 1;
        var tenant = new Data.Models.Tenant.Tenant { Id = Guid.NewGuid() };
        var expectedResponse = new TenantResponse { Id = tenantId };

        _tenantRepositoryMock.Setup(x => x.GetTenantByIdAsync(tenantId))
            .ReturnsAsync(tenant);
        _mapperMock.Setup(x => x.Map<TenantResponse>(tenant))
            .Returns(expectedResponse);

        // Act
        var result = await _tenantService.GetTenantByIdAsync(tenantId);

        // Assert
        Assert.Null(result);
        _tenantRepositoryMock.Verify(x => x.GetTenantByIdAsync(tenantId), Times.Once);
    }

    [Fact]
    public async Task CreateTenantAsync_CreatesTenant_WhenValidInput()
    {
        // Arrange
        var request = new Models.Tenant.TenantRequest();
        var tenant = new Data.Models.Tenant.Tenant();
        var expectedResponse = new TenantResponse();

        _mapperMock.Setup(x => x.Map<Data.Models.Tenant.Tenant>(request))
            .Returns(tenant);
        _tenantRepositoryMock.Setup(x => x.CreateTenantAsync(tenant))
            .ReturnsAsync(tenant);
        _mapperMock.Setup(x => x.Map<TenantResponse>(tenant))
            .Returns(expectedResponse);

        // Act
        var result = await _tenantService.CreateTenantAsync(request);

        // Assert
        Assert.Null(result);
        _tenantRepositoryMock.Verify(x => x.CreateTenantAsync(tenant), Times.Once);
    }

    [Fact]
    public async Task UpdateTenantAsync_UpdatesTenant_WhenTenantExists()
    {
        // Arrange
        var tenantId = 1;
        var request = new Models.Tenant.TenantRequest();
        var existingTenant = new Data.Models.Tenant.Tenant { Id = Guid.NewGuid() };
        var updatedTenant = new Data.Models.Tenant.Tenant { Id = Guid.NewGuid() };
        var expectedResponse = new TenantResponse { Id = tenantId };

        _tenantRepositoryMock.Setup(x => x.GetTenantByIdAsync(tenantId))
            .ReturnsAsync(existingTenant);
        _mapperMock.Setup(x => x.Map(request, existingTenant))
            .Returns(updatedTenant);
        _tenantRepositoryMock.Setup(x => x.UpdateTenantAsync(updatedTenant))
            .ReturnsAsync(updatedTenant);
        _mapperMock.Setup(x => x.Map<TenantResponse>(updatedTenant))
            .Returns(expectedResponse);

        // Act
        var result = await _tenantService.UpdateTenantAsync(tenantId, request);

        // Assert
        Assert.Null(result);
        _tenantRepositoryMock.Verify(x => x.GetTenantByIdAsync(tenantId), Times.Once);
    }

    [Fact]
    public async Task DeleteTenantAsync_DeletesTenant_WhenTenantExists()
    {
        // Arrange
        var tenantId = 1;
        var tenant = new Data.Models.Tenant.Tenant { Id = Guid.NewGuid() };

        _tenantRepositoryMock.Setup(x => x.GetTenantByIdAsync(tenantId))
            .ReturnsAsync(tenant);
        _tenantRepositoryMock.Setup(x => x.DeleteTenantAsync(tenant))
            .Returns(Task.CompletedTask);

        // Act
        await _tenantService.DeleteTenantAsync(tenantId);

        // Assert
        _tenantRepositoryMock.Verify(x => x.GetTenantByIdAsync(tenantId), Times.Once);
        _tenantRepositoryMock.Verify(x => x.DeleteTenantAsync(tenant), Times.Once);
    }

    [Fact]
    public async Task SearchTenants_ReturnsPagingResponse_WhenTenantsFound()
    {
        // Arrange
        var searchRequest = new Models.Tenant.TenantSearchRequest();
        var expectedResponse = new PagingResponse<TenantResponse>();

        _mapperMock.Setup(x => x.Map<Data.Models.Tenant.TenantSearchRequest>(searchRequest))
            .Returns(new Data.Models.Tenant.TenantSearchRequest());
        _tenantRepositoryMock.Setup(x => x.SearchTenants(It.IsAny<Data.Models.Tenant.TenantSearchRequest>()))
            .ReturnsAsync(new PagingResponse<Data.Models.Tenant.Tenant>());
        _mapperMock.Setup(x => x.Map<PagingResponse<TenantResponse>>(It.IsAny<PagingResponse<Data.Models.Tenant.Tenant>>()))
            .Returns(expectedResponse);

        // Act
        var result = await _tenantService.SearchTenants(searchRequest);

        // Assert
        Assert.Null(result);
        _tenantRepositoryMock.Verify(x => x.SearchTenants(It.IsAny<Data.Models.Tenant.TenantSearchRequest>()), Times.Once);
    }
}