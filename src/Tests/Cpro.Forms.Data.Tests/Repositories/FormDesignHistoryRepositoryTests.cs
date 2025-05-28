using Cpro.Forms.Data.Infrastructure;
using Cpro.Forms.Data.Models;
using Cpro.Forms.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cpro.Forms.Data.Tests.Repositories;

public class FormDesignHistoryRepositoryTests
{
    private readonly SqlContext _dbContext;
    private readonly IFormDesignHistoryRepository _repository;

    public FormDesignHistoryRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<SqlContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new SqlContext(options);
        _repository = new FormDesignHistoryRepository(_dbContext);
    }

    [Fact]
    public async Task SaveFormDesignHistoryAsync_ValidInput_SavesSuccessfully()
    {
        // Arrange
        var formDesign = new FormDesignHistory
        {
            Id = Guid.NewGuid().ToString(),
            FormDesignId = "form-123",
            Version = 1,
            Name = "Initial Version",
            TenantName = "Test Tenant",
            StorageUrl = "http://example.com/storage",
        };

        // Act
        var result = await _repository.SaveFormDesignHistoryAsync(formDesign);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("form-123", result.FormDesignId);
        Assert.True(result.DateCreated <= DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task GetAllVersions_ExistingFormId_ReturnsVersionsDescending()
    {
        // Arrange
        var formId = "form-456";
        var versions = new List<FormDesignHistory>
        {
            new FormDesignHistory { Id = Guid.NewGuid().ToString(), FormDesignId = formId, Version = 1, Name = "Initial Version", TenantName = "Test Tenant", StorageUrl = "http://example.com/storage" },
            new FormDesignHistory { Id = Guid.NewGuid().ToString(), FormDesignId = formId, Version = 2, Name = "Initial Version", TenantName = "Test Tenant", StorageUrl = "http://example.com/storage" },
            new FormDesignHistory { Id = Guid.NewGuid().ToString(), FormDesignId = formId, Version = 3, Name = "Initial Version", TenantName = "Test Tenant", StorageUrl = "http://example.com/storage" },
        };

        _dbContext.FormDesignsHistory.AddRange(versions);
        await _dbContext.SaveChangesAsync();

        // Detach to avoid tracking conflicts
        foreach (var v in versions)
        {
            _dbContext.Entry(v).State = EntityState.Detached;
        }

        // Act
        var results = await _repository.GetAllVersions(formId);

        // Assert
        Assert.Equal(3, results.Count);
        Assert.Equal(3, results[0].Version); // Check descending order
        Assert.Equal(2, results[1].Version);
        Assert.Equal(1, results[2].Version);
    }

    [Fact]
    public async Task GetVersion_ExistingVersion_ReturnsCorrectHistory()
    {
        // Arrange
        var formId = "form-789";
        var targetVersion = 5;
        var expected = new FormDesignHistory
        {
            Id = Guid.NewGuid().ToString(),
            FormDesignId = formId,
            Version = targetVersion,
            Name = "Version 5",
            TenantName = "Test Tenant",
            StorageUrl = "http://example.com/storage",
        };

        _dbContext.FormDesignsHistory.Add(expected);
        await _dbContext.SaveChangesAsync();

        _dbContext.Entry(expected).State = EntityState.Detached;

        // Act
        var result = await _repository.GetVersion(formId, targetVersion);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(formId, result.FormDesignId);
        Assert.Equal(targetVersion, result.Version);
    }

    [Fact]
    public async Task GetVersion_NonExistent_ReturnsNull()
    {
        // Act
        var result = await _repository.GetVersion("non-existent-form", 999);

        // Assert
        Assert.Null(result);
    }
}