using Cpro.Forms.Data.Infrastructure;
using Cpro.Forms.Data.Models;
using Cpro.Forms.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cpro.Forms.Data.Tests.Repositories;

public class FormTemplateRepositoryTests
{
    private readonly SqlContext _dbContext;
    private readonly IFormTemplateRepository _repository;

    public FormTemplateRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<SqlContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new SqlContext(options);
        _repository = new FormTemplateRepository(_dbContext);
    }

    [Fact]
    public async Task CreateFormTemplateAsync_ValidTemplate_SavesSuccessfully()
    {
        // Arrange
        var template = new FormTemplate
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Template A"
        };

        // Act
        var result = await _repository.CreateFormTemplateAsync(template);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Template A", result.Name);
    }

    [Fact]
    public async Task GetFormTemplate_ExistingId_ReturnsTemplate()
    {
        // Arrange
        var templateId = Guid.NewGuid().ToString();
        var template = new FormTemplate
        {
            Id = templateId,
            Name = "Retrieve Me"
        };

        _dbContext.FormTemplates.Add(template);
        await _dbContext.SaveChangesAsync();
        _dbContext.Entry(template).State = EntityState.Detached;

        // Act
        var result = await _repository.GetFormTemplate(templateId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Retrieve Me", result.Name);
    }

    [Fact]
    public async Task UpdateFormTemplateAsync_ExistingId_UpdatesSuccessfully()
    {
        // Arrange
        var templateId = "update-template";
        var template = new FormTemplate
        {
            Id = templateId,
            Name = "Old Name"
        };

        _dbContext.FormTemplates.Add(template);
        await _dbContext.SaveChangesAsync();
        _dbContext.Entry(template).State = EntityState.Detached;

        template.Name = "Updated Name";

        // Act
        var result = await _repository.UpdateFormTemplateAsync(templateId, template);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Name);
    }

    [Fact]
    public async Task DeleteFormTemplateAsync_ExistingId_DeletesSuccessfully()
    {
        // Arrange
        var templateId = "delete-me";
        var template = new FormTemplate
        {
            Id = templateId,
            Name = "To Be Deleted"
        };

        _dbContext.FormTemplates.Add(template);
        await _dbContext.SaveChangesAsync();
        _dbContext.Entry(template).State = EntityState.Detached;

        // Act
        var result = await _repository.DeleteFormTemplateAsync(templateId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(templateId, result.Id);
        Assert.Null(await _dbContext.FormTemplates.FindAsync(templateId));
    }

    [Fact]
    public async Task SearchFormTemplatesAsync_WithKeyword_ReturnsMatchingTemplates()
    {
        // Arrange
        _dbContext.FormTemplates.AddRange(
            new FormTemplate { Id = "1", Name = "Onboarding" },
            new FormTemplate { Id = "2", Name = "Exit Survey" },
            new FormTemplate { Id = "3", Name = "HR Onboarding Checklist" }
        );

        await _dbContext.SaveChangesAsync();

        var searchRequest = new SearchRequest
        {
            Page = 1,
            PageSize = 10,
            Keyword = "Onboarding"
        };

        // Act
        var result = await _repository.SearchFormTemplatesAsync(searchRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Data, t => Assert.Contains("Onboarding", t.Name));
    }

    [Fact]
    public async Task GetFormTemplate_NonExistentId_ReturnsNull()
    {
        // Act
        var result = await _repository.GetFormTemplate("non-existent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteFormTemplateAsync_NonExistentId_ReturnsNull()
    {
        // Act
        var result = await _repository.DeleteFormTemplateAsync("non-existent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateFormTemplateAsync_NonExistentId_ReturnsNull()
    {
        // Arrange
        var fakeTemplate = new FormTemplate
        {
            Id = "non-existent",
            Name = "Doesn't Matter"
        };

        // Act
        var result = await _repository.UpdateFormTemplateAsync("non-existent", fakeTemplate);

        // Assert
        Assert.Null(result);
    }
}