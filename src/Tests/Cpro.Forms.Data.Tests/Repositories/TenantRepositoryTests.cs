using Cpro.Forms.Data.Infrastructure;
using Cpro.Forms.Data.Models.Tenant;
using Cpro.Forms.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cpro.Forms.Data.Tests.Repositories;

public class TenantRepositoryTests
{
    private readonly SqlContext _dbContext;
    private readonly ITenantRepository _repository;

    public TenantRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<SqlContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new SqlContext(options);
        _repository = new TenantRepository(_dbContext);
    }

    [Fact]
    public async Task CreateTenantAsync_ValidTenant_SavesSuccessfully()
    {
        // Arrange
        var tenant = new Tenant
        {
            TenantId = 1,
            TenantName = "Acme Corp"
        };

        // Act
        var result = await _repository.CreateTenantAsync(tenant);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Acme Corp", result.TenantName);
    }

    [Fact]
    public async Task GetTenantByIdAsync_ExistingTenantId_ReturnsTenant()
    {
        // Arrange
        var tenant = new Tenant
        {
            TenantId = 42,
            TenantName = "Globex"
        };

        _dbContext.Tenants.Add(tenant);
        await _dbContext.SaveChangesAsync();
        _dbContext.Entry(tenant).State = EntityState.Detached;

        // Act
        var result = await _repository.GetTenantByIdAsync(42);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Globex", result.TenantName);
    }

    [Fact]
    public async Task UpdateTenantAsync_ExistingTenant_UpdatesSuccessfully()
    {
        // Arrange
        var tenant = new Tenant
        {
            TenantId = 5,
            TenantName = "Old Name"
        };

        _dbContext.Tenants.Add(tenant);
        await _dbContext.SaveChangesAsync();
        _dbContext.Entry(tenant).State = EntityState.Detached;

        tenant.TenantName = "New Name";

        // Act
        var result = await _repository.UpdateTenantAsync(tenant);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Name", result.TenantName);
    }

    [Fact]
    public async Task SearchTenants_WithMatchingName_ReturnsPagedResult()
    {
        // Arrange
        _dbContext.Tenants.AddRange(
            new Tenant { TenantId = 1, TenantName = "Contoso Ltd" },
            new Tenant { TenantId = 2, TenantName = "Contoso USA" },
            new Tenant { TenantId = 3, TenantName = "Fabrikam" }
        );

        await _dbContext.SaveChangesAsync();

        var searchRequest = new TenantSearchRequest
        {
            TenantName = "Contoso",
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await _repository.SearchTenants(searchRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Data, t => Assert.Contains("Contoso", t.TenantName));
    }

    [Fact]
    public async Task GetTenantByIdAsync_NonExistingId_ReturnsNull()
    {
        // Act
        var result = await _repository.GetTenantByIdAsync(9999);

        // Assert
        Assert.Null(result);
    }
}
