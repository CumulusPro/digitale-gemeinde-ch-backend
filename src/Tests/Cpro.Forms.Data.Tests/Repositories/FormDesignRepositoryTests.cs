using Cpro.Forms.Data.Infrastructure;
using Cpro.Forms.Data.Models;
using Cpro.Forms.Data.Models.User;
using Cpro.Forms.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cpro.Forms.Data.Tests.Repositories;

public class FormDesignRepositoryTests
{
    private readonly SqlContext _dbContext;
    private readonly IFormDesignRepository _repository;

    public FormDesignRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<SqlContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new SqlContext(options);
        _repository = new FormDesignRepository(_dbContext);
    }

    [Fact]
    public async Task CreateFormDesignAsync_ValidInput_SavesSuccessfully()
    {
        // Arrange
        var tag = new Tag { TagName = "TagA" };
        await _dbContext.Tags.AddAsync(tag);
        await _dbContext.SaveChangesAsync();

        var formDesign = new FormDesign
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Form A",
            TenantId = 1,
            Tags = new List<FormDesignTag>
            {
                new FormDesignTag
                {
                    TagId = tag.TagId
                }
            },
            TenantName = "Test Tenant",
        };

        // Act
        var result = await _repository.CreateFormDesignAsync(formDesign);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Form A", result.Name);
        Assert.True(result.DateCreated <= DateTimeOffset.UtcNow);
        Assert.Single(result.Tags);
        Assert.Equal("TagA", result.Tags.First().Tag.TagName);
    }

    [Fact]
    public async Task GetFormDesign_ValidTenantId_ReturnsDesign()
    {
        // Arrange
        var formId = Guid.NewGuid().ToString();
        var formDesign = new FormDesign
        {
            Id = formId,
            Name = "Tenant Form",
            TenantId = 42,
            TenantName = "Test Tenant",
        };

        _dbContext.FormDesigns.Add(formDesign);
        await _dbContext.SaveChangesAsync();
        _dbContext.Entry(formDesign).State = EntityState.Detached;

        // Act
        var result = await _repository.GetFormDesign(formId, 42);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Tenant Form", result.Name);
    }

    [Fact]
    public async Task GetFormDesign_InValidTenantId_ReturnsDesign()
    {
        // Arrange
        var formId = Guid.NewGuid().ToString();
        var formDesign = new FormDesign
        {
            Id = formId,
            Name = "Tenant Form",
            TenantId = -1,
            TenantName = "Test Tenant",
        };

        _dbContext.FormDesigns.Add(formDesign);
        await _dbContext.SaveChangesAsync();
        _dbContext.Entry(formDesign).State = EntityState.Detached;

        // Act
        var result = await _repository.GetFormDesign(formId, -1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Tenant Form", result.Name);
    }

    [Fact]
    public async Task GetFormDesignByFormId_ValidFormId_ReturnsDesign()
    {
        // Arrange
        var formId = "form-xyz";
        var formDesign = new FormDesign
        {
            Id = formId,
            Name = "XYZ Form",
            TenantId = 1,
            TenantName = "Test Tenant",
        };

        _dbContext.FormDesigns.Add(formDesign);
        await _dbContext.SaveChangesAsync();
        _dbContext.Entry(formDesign).State = EntityState.Detached;

        // Act
        var result = await _repository.GetFormDesignByFormId(formId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("XYZ Form", result.Name);
    }

    [Fact]
    public async Task GetFormDesignsByTenantId_ExistingTenant_ReturnsList()
    {
        // Arrange
        var tenantId = 101;
        _dbContext.FormDesigns.Add(new FormDesign
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Tenant Form A",
            TenantId = tenantId,
            TenantName = "Test Tenant",
        });

        await _dbContext.SaveChangesAsync();

        // Act
        var results = await _repository.GetFormDesignsByTenantId(tenantId);

        // Assert
        Assert.Single(results);
        Assert.Equal("Tenant Form A", results[0].Name);
    }

    [Fact]
    public async Task GetFormDesignCountAsync_ReturnsCorrectCount()
    {
        // Arrange
        _dbContext.FormDesigns.AddRange(
            new FormDesign { Id = "1", Name = "Form 1", TenantId = 1, TenantName = "Test Tenant", },
            new FormDesign { Id = "2", Name = "Form 2", TenantId = 2, TenantName = "Test Tenant", }
        );

        await _dbContext.SaveChangesAsync();

        // Act
        var count = await _repository.GetFormDesignCountAsync();

        // Assert
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task DeleteFormDesignAsync_ValidFormId_DeletesSuccessfully()
    {
        // Arrange
        var formId = "delete-me";
        var tenantId = 5;

        var formDesign = new FormDesign
        {
            Id = formId,
            Name = "Delete Me",
            TenantId = tenantId,
            TenantName = "Test Tenant",
        };

        _dbContext.FormDesigns.Add(formDesign);
        await _dbContext.SaveChangesAsync();
        _dbContext.Entry(formDesign).State = EntityState.Detached;

        // Act
        var result = await _repository.DeleteFormDesignAsync(formId, tenantId);

        // Assert
        Assert.NotNull(result);
        Assert.Null(await _dbContext.FormDesigns.FindAsync(formId));
    }

    [Fact]
    public async Task SearchFormDesignsAsync_AsDesigner_SeesOnlyAssignedDesigns()
    {
        // Arrange
        var tenantId = 1;
        var designerEmail = "designer@test.com";

        var user = new User { Email = designerEmail, Role = Role.Designer, TenantId = tenantId };
        await _dbContext.Users.AddAsync(user);

        var formDesign = new FormDesign
        {
            Id = "search-2",
            Name = "Employee Info",
            TenantId = tenantId,
            TenantName = "Test Tenant",
            Designers = new List<Designer>
            {
                new Designer { Email = designerEmail }
            }
        };

        await _dbContext.FormDesigns.AddAsync(formDesign);
        await _dbContext.SaveChangesAsync();

        var searchRequest = new SearchRequest
        {
            Page = 1,
            PageSize = 10,
            Keyword = "Employee"
        };

        // Act
        var result = await _repository.SearchFormDesignsAsync(searchRequest, tenantId, designerEmail);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TotalCount);
        Assert.Single(result.Data);
        Assert.Equal("Employee Info", result.Data.First().Name);
    }

    [Fact]
    public async Task SearchFormDesignsAsync_AsAdmin_SeesAllMatchingForms()
    {
        // Arrange
        var tenantId = 1;
        var adminEmail = "admin@test.com";

        var user = new User { Email = adminEmail, Role = Role.Admin, TenantId = tenantId };
        await _dbContext.Users.AddAsync(user);

        var tag = new Tag { TagName = "HR" };
        await _dbContext.Tags.AddAsync(tag);
        await _dbContext.SaveChangesAsync();

        var formDesign = new FormDesign
        {
            Id = "search-1",
            Name = "Onboarding Form",
            TenantId = tenantId,
            TenantName = "Test Tenant",
            Tags = new List<FormDesignTag>
            {
                new FormDesignTag { TagId = tag.TagId }
            }
        };

        await _dbContext.FormDesigns.AddAsync(formDesign);
        await _dbContext.SaveChangesAsync();

        var searchRequest = new SearchRequest
        {
            Page = 1,
            PageSize = 10,
            Keyword = "Onboarding"
        };

        // Act
        var result = await _repository.SearchFormDesignsAsync(searchRequest, tenantId, adminEmail);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TotalCount);
        Assert.Single(result.Data);
        Assert.Equal("Onboarding Form", result.Data.First().Name);
    }

    [Fact]
    public async Task SearchFormDesignsAsync_AsProcessor_SeesOnlyAssignedDesigns()
    {
        // Arrange
        var tenantId = 1;
        var processorEmail = "processor@test.com";

        var user = new User { Email = processorEmail, Role = Role.Processor, TenantId = tenantId };
        await _dbContext.Users.AddAsync(user);

        var formDesign = new FormDesign
        {
            Id = "search-3",
            Name = "Payroll Form",
            TenantId = tenantId,
            TenantName = "Test Tenant",
            Processors = new List<Processor>
            {
                new Processor { Email = processorEmail }
            }
        };

        await _dbContext.FormDesigns.AddAsync(formDesign);
        await _dbContext.SaveChangesAsync();

        var searchRequest = new SearchRequest
        {
            Page = 1,
            PageSize = 10,
            Keyword = "Payroll"
        };

        // Act
        var result = await _repository.SearchFormDesignsAsync(searchRequest, tenantId, processorEmail);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TotalCount);
        Assert.Single(result.Data);
        Assert.Equal("Payroll Form", result.Data.First().Name);
    }

    [Fact]
    public async Task UpdateFormDesignAsync_ValidInput_UpdatesSuccessfully()
    {
        // Arrange
        var formId = "update-1";
        var formDesign = new FormDesign
        {
            Id = formId,
            Name = "Old Name",
            TenantId = 1,
            TenantName = "Test Tenant",
        };

        _dbContext.FormDesigns.Add(formDesign);
        await _dbContext.SaveChangesAsync();
        _dbContext.Entry(formDesign).State = EntityState.Detached;

        formDesign.Name = "Updated Name";

        // Act
        var result = await _repository.UpdateFormDesignAsync(formId, formDesign);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Name);
        Assert.True(result.DateUpdated <= DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task GetTagsByNamesAsync_ExistingNames_ReturnsMatchingTags()
    {
        // Arrange
        var tag1 = new Tag { TagName = "Tag1" };
        var tag2 = new Tag { TagName = "Tag2" };
        await _dbContext.Tags.AddRangeAsync(tag1, tag2);
        await _dbContext.SaveChangesAsync();

        var tagNames = new List<string> { "Tag1", "Tag2" };

        // Act
        var result = await _repository.GetTagsByNamesAsync(tagNames);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, t => t.TagName == "Tag1");
        Assert.Contains(result, t => t.TagName == "Tag2");
    }

    [Fact]
    public async Task AddTagsAsync_ValidTags_AddsSuccessfully()
    {
        // Arrange
        var newTags = new List<Tag>
        {
            new Tag { TagName = "NewTag1" },
            new Tag { TagName = "NewTag2" }
        };

        // Act
        await _repository.AddTagsAsync(newTags);

        // Assert
        var savedTags = await _dbContext.Tags.ToListAsync();
        Assert.Equal(2, savedTags.Count);
        Assert.Contains(savedTags, t => t.TagName == "NewTag1");
        Assert.Contains(savedTags, t => t.TagName == "NewTag2");
    }

    [Fact]
    public async Task GetAllDistinctTagNamesAsync_ReturnsSortedTagNames()
    {
        // Arrange
        var tags = new List<Tag>
        {
            new Tag { TagName = "Zebra" },
            new Tag { TagName = "Apple" },
            new Tag { TagName = "Mango" }
        };

        await _dbContext.Tags.AddRangeAsync(tags);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllDistinctTagNamesAsync();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal(new List<string> { "Apple", "Mango", "Zebra" }, result);
    }

    [Fact]
    public async Task RemoveUserFromFormDesigns_RemovesUserFromDesignsAndSavesChanges()
    {
        // Arrange
        var userEmail = "remove@example.com";
        var formId1 = "form-1";
        var formId2 = "form-2";
        var tenantId = 1;

        var form1 = new FormDesign
        {
            Id = formId1,
            Name = "Form 1",
            TenantId = tenantId,
            TenantName = "Tenant 1",
            Designers = new List<Designer> { new Designer { Email = userEmail, FormDesignId = formId1 } },
            Processors = new List<Processor> { new Processor { Email = "other@example.com", FormDesignId = formId1 } },
            DateCreated = DateTimeOffset.UtcNow,
            DateUpdated = DateTimeOffset.UtcNow
        };

        var form2 = new FormDesign
        {
            Id = formId2,
            Name = "Form 2",
            TenantId = tenantId,
            TenantName = "Tenant 2",
            Designers = new List<Designer> { new Designer { Email = "other@example.com", FormDesignId = formId2 } },
            Processors = new List<Processor> { new Processor { Email = userEmail, FormDesignId = formId2 } },
            DateCreated = DateTimeOffset.UtcNow,
            DateUpdated = DateTimeOffset.UtcNow
        };

        _dbContext.FormDesigns.AddRange(form1, form2);
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();

        // Act
        await _repository.RemoveUserFromFormDesigns(userEmail, tenantId);

        // Assert
        var updatedForm1 = await _dbContext.FormDesigns
            .Include(f => f.Designers)
            .Include(f => f.Processors)
            .FirstOrDefaultAsync(f => f.Id == formId1);

        var updatedForm2 = await _dbContext.FormDesigns
            .Include(f => f.Designers)
            .Include(f => f.Processors)
            .FirstOrDefaultAsync(f => f.Id == formId2);

        Assert.NotNull(updatedForm1);
        Assert.DoesNotContain(updatedForm1.Designers, d => d.Email == userEmail);
        Assert.Contains(updatedForm1.Processors, p => p.Email == "other@example.com");

        Assert.NotNull(updatedForm2);
        Assert.DoesNotContain(updatedForm2.Processors, p => p.Email == userEmail);
        Assert.Contains(updatedForm2.Designers, d => d.Email == "other@example.com");

        Assert.True(updatedForm1.DateUpdated > form1.DateUpdated);
        Assert.True(updatedForm2.DateUpdated > form2.DateUpdated);
    }

}