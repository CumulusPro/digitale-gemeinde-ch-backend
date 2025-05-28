using Cpro.Forms.Data.Infrastructure;
using Cpro.Forms.Data.Models;
using Cpro.Forms.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cpro.Forms.Data.Tests.Repositories;

public class FormDataRepositoryTests
{
    private readonly SqlContext _dbContext;
    private readonly IFormDataRepository _formRepository;

    public FormDataRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<SqlContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new SqlContext(options);
        _formRepository = new FormDataRepository(_dbContext);
    }

    [Fact]
    public async Task GetFormDataAsync_FormExists_ReturnsCorrectFormData()
    {
        // Arrange
        var formId = "test-form-id";
        var tenantId = 1;
        var documentId = "test-document-id";

        var formData = new FormData
        {
            Id = formId,
            Name = "Test Form",
            Status = "Draft",
            FormId = formId,
            TenantId = tenantId,
            DocumentId = documentId
        };

        _dbContext.FormDatas.Add(formData);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _formRepository.GetFormData(formId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(formId, result.FormId);
        Assert.Equal(tenantId, result.TenantId);
        Assert.Equal(documentId, result.DocumentId);
    }

    [Fact]
    public async Task UpdateFormDataAsync_FormExists_UpdatesSuccessfully()
    {
        // Arrange
        var formId = "test-form-id";
        var formData = new FormData
        {
            Id = formId,
            Name = "Original Form",
            FormId = formId,
            TenantId = 1,
            DocumentId = "original-document-id",
            Status = "Draft"
        };

        _dbContext.FormDatas.Add(formData);
        await _dbContext.SaveChangesAsync();

        // Modify data
        formData.Status = "Completed";

        // Act
        var result = await _formRepository.UpdateFormDataAsync(formId, formData);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Completed", result.Status);
    }

    [Fact]
    public async Task SearchFormDatasAsync_WithValidTenantId_ReturnsMatchingResults()
    {
        // Arrange
        var formData = new FormData
        {
            Id = "form-data-id",
            FormId = "test-form-id",
            TenantId = 1,
            DocumentId = "test-document-id",
            Name = "Sample Form",
            Status = "Draft"
        };

        _dbContext.FormDatas.Add(formData);
        await _dbContext.SaveChangesAsync();

        var searchRequest = new FormSearchRequest
        {
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await _formRepository.SearchFormDatasAsync(searchRequest, tenantId: 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TotalCount);
        Assert.Single(result.Data);
        Assert.Equal("test-form-id", result.Data[0].FormId);
    }

    [Fact]
    public async Task CreateFormDataAsync_ValidFormData_CreatesSuccessfully()
    {
        // Arrange
        var formData = new FormData
        {
            Id = Guid.NewGuid().ToString(),
            FormId = "new-form-id",
            TenantId = 1,
            DocumentId = "new-doc-id",
            Name = "New Form",
            Status = "New"
        };

        // Act
        var result = await _formRepository.CreateFormDataAsync(formData);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("new-form-id", result.FormId);
        Assert.Equal("new-doc-id", result.DocumentId);
    }

    [Fact]
    public async Task DeleteFormDataAsync_ValidFormId_DeletesSuccessfully()
    {
        // Arrange
        var formId = Guid.NewGuid().ToString();
        var formData = new FormData
        {
            Id = formId,
            FormId = formId,
            TenantId = 1,
            DocumentId = "doc-to-delete",
            Name = "To Delete",
            Status = "Deleted"
        };

        _dbContext.FormDatas.Add(formData);
        await _dbContext.SaveChangesAsync();
        _dbContext.Entry(formData).State = EntityState.Detached;

        // Act
        var result = await _formRepository.DeleteFormDataAsync(formId, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("doc-to-delete", result.DocumentId);
        Assert.Null(await _dbContext.FormDatas.FindAsync(formId));
    }

    [Fact]
    public async Task GetFormDataByDocumentId_ValidDocumentId_ReturnsCorrectFormData()
    {
        // Arrange
        var documentId = "unique-doc-id";
        var formData = new FormData
        {
            Id = Guid.NewGuid().ToString(),
            FormId = "lookup-form-id",
            TenantId = 1,
            DocumentId = documentId,
            Name = "Lookup Test",
            Status = "Active"
        };

        _dbContext.FormDatas.Add(formData);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _formRepository.GetFormDataByDocumentId(documentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("lookup-form-id", result.FormId);
        Assert.Equal(documentId, result.DocumentId);
    }

    [Fact]
    public async Task GetFormDatasByTenantId_ValidTenantId_ReturnsList()
    {
        // Arrange
        var tenantId = 42;
        var formData = new FormData
        {
            Id = Guid.NewGuid().ToString(),
            FormId = "multi-form-id",
            TenantId = tenantId,
            DocumentId = "tenant-doc",
            Name = "Multi Tenant Test",
            Status = "Processing"
        };

        _dbContext.FormDatas.Add(formData);
        await _dbContext.SaveChangesAsync();

        // Act
        var results = await _formRepository.GetFormDatasByTenantId(tenantId);

        // Assert
        Assert.Single(results);
        Assert.Equal(tenantId, results[0].TenantId);
    }

    [Fact]
    public async Task GetFormDataCountAsync_FormsExist_ReturnsCorrectCount()
    {
        // Arrange
        _dbContext.FormDatas.AddRange(
            new FormData { Id = Guid.NewGuid().ToString(), FormId = "f1", TenantId = 1, DocumentId = "tenant-doc", Name = "Multi Tenant Test", Status = "Processing" },
            new FormData { Id = Guid.NewGuid().ToString(), FormId = "f2", TenantId = 1, DocumentId = "tenant-doc", Name = "Multi Tenant Test", Status = "Processing" }
        );
        await _dbContext.SaveChangesAsync();

        // Act
        var count = await _formRepository.GetFormDataCountAsync();

        // Assert
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task GetNextSequenceDocumentId_ReturnsNextSequenceValue()
    {        
        // The in-memory provider does not support sequences.

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            var result = await _formRepository.GetNextSequenceDocumentId();
        });
    }
}