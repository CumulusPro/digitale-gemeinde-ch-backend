//using Cpro.Forms.Data.Infrastructure;
//using Cpro.Forms.Data.Models;
//using Cpro.Forms.Data.Repositories;
//using Microsoft.EntityFrameworkCore;
//using Moq;

//namespace Cpro.Forms.Data.Tests.Repositories;

//public class FormRepositoryTests
//{
//    private readonly Mock<SqlContext> _dbContextMock;
//    private readonly IFormDataRepository _formRepository;

//    public FormRepositoryTests()
//    {
//        var options = new DbContextOptionsBuilder<SqlContext>()
//            .UseInMemoryDatabase(databaseName: "TestDb")
//            .Options;
//        _dbContextMock = new Mock<SqlContext>(options);
//        _formRepository = new FormDataRepository(_dbContextMock.Object);
//    }

//    [Fact]
//    public async Task GetFormDataAsync_ReturnsDocumentResponse_WhenFormExists()
//    {
//        // Arrange
//        var formId = "test-form-id";
//        var tenantId = 1;
//        var documentId = "test-document-id";
//        var expectedResponse = new DocumentResponse();
//        var formData = new FormData
//        {
//            FormId = formId,
//            TenantId = tenantId,
//            DocumentId = documentId,
//            Data = "{}"
//        };

//        var dbSetMock = new Mock<DbSet<FormData>>();
//        dbSetMock.As<IQueryable<FormData>>()
//            .Setup(m => m.Provider)
//            .Returns(new[] { formData }.AsQueryable().Provider);
//        dbSetMock.As<IQueryable<FormData>>()
//            .Setup(m => m.Expression)
//            .Returns(new[] { formData }.AsQueryable().Expression);
//        dbSetMock.As<IQueryable<FormData>>()
//            .Setup(m => m.ElementType)
//            .Returns(new[] { formData }.AsQueryable().ElementType);
//        dbSetMock.As<IQueryable<FormData>>()
//            .Setup(m => m.GetEnumerator())
//            .Returns(new[] { formData }.AsQueryable().GetEnumerator());

//        _dbContextMock.Setup(x => x.FormData)
//            .Returns(dbSetMock.Object);

//        // Act
//        var result = await _formRepository.GetFormDataAsync(formId, tenantId, documentId);

//        // Assert
//        Assert.NotNull(result);
//        Assert.Equal(formId, result.FormId);
//        Assert.Equal(tenantId, result.TenantId);
//        Assert.Equal(documentId, result.DocumentId);
//    }

//    [Fact]
//    public async Task SubmitTaskAsync_ReturnsFormResponse_WhenSubmissionSuccessful()
//    {
//        // Arrange
//        var jsonData = new { };
//        var origin = "test-origin";
//        var expectedResponse = new FormResponse();
//        var formData = new FormData
//        {
//            FormId = "test-form-id",
//            TenantId = 1,
//            DocumentId = "test-document-id",
//            Data = "{}"
//        };

//        var dbSetMock = new Mock<DbSet<FormData>>();
//        _dbContextMock.Setup(x => x.FormData)
//            .Returns(dbSetMock.Object);

//        // Act
//        var result = await _formRepository.SubmitTaskAsync(jsonData, origin);

//        // Assert
//        Assert.NotNull(result);
//        dbSetMock.Verify(x => x.Add(It.IsAny<FormData>()), Times.Once);
//        _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//    }

//    [Fact]
//    public async Task UpdateFormStatus_ReturnsCreateFormDefinitionResponse_WhenUpdateSuccessful()
//    {
//        // Arrange
//        var tenantId = 1;
//        var formId = "test-form-id";
//        var documentId = "test-document-id";
//        var status = "Completed";
//        var formData = new FormData
//        {
//            FormId = formId,
//            TenantId = tenantId,
//            DocumentId = documentId,
//            Status = status
//        };

//        var dbSetMock = new Mock<DbSet<FormData>>();
//        dbSetMock.As<IQueryable<FormData>>()
//            .Setup(m => m.Provider)
//            .Returns(new[] { formData }.AsQueryable().Provider);
//        dbSetMock.As<IQueryable<FormData>>()
//            .Setup(m => m.Expression)
//            .Returns(new[] { formData }.AsQueryable().Expression);
//        dbSetMock.As<IQueryable<FormData>>()
//            .Setup(m => m.ElementType)
//            .Returns(new[] { formData }.AsQueryable().ElementType);
//        dbSetMock.As<IQueryable<FormData>>()
//            .Setup(m => m.GetEnumerator())
//            .Returns(new[] { formData }.AsQueryable().GetEnumerator());

//        _dbContextMock.Setup(x => x.FormData)
//            .Returns(dbSetMock.Object);

//        // Act
//        var result = await _formRepository.UpdateFormStatus(tenantId, formId, documentId, status);

//        // Assert
//        Assert.NotNull(result);
//        _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//    }

//    [Fact]
//    public async Task GetFormNavigationAsync_ReturnsFormNavigationList_WhenNavigationExists()
//    {
//        // Arrange
//        var tenantId = 1;
//        var formNavigation = new FormNavigation
//        {
//            TenantId = tenantId,
//            FormId = "test-form-id",
//            Name = "Test Form"
//        };

//        var dbSetMock = new Mock<DbSet<FormNavigation>>();
//        dbSetMock.As<IQueryable<FormNavigation>>()
//            .Setup(m => m.Provider)
//            .Returns(new[] { formNavigation }.AsQueryable().Provider);
//        dbSetMock.As<IQueryable<FormNavigation>>()
//            .Setup(m => m.Expression)
//            .Returns(new[] { formNavigation }.AsQueryable().Expression);
//        dbSetMock.As<IQueryable<FormNavigation>>()
//            .Setup(m => m.ElementType)
//            .Returns(new[] { formNavigation }.AsQueryable().ElementType);
//        dbSetMock.As<IQueryable<FormNavigation>>()
//            .Setup(m => m.GetEnumerator())
//            .Returns(new[] { formNavigation }.AsQueryable().GetEnumerator());

//        _dbContextMock.Setup(x => x.FormNavigation)
//            .Returns(dbSetMock.Object);

//        // Act
//        var result = await _formRepository.GetFormNavigationAsync(tenantId);

//        // Assert
//        Assert.NotNull(result);
//        Assert.Single(result);
//        Assert.Equal(tenantId, result[0].TenantId);
//        Assert.Equal("test-form-id", result[0].FormId);
//        Assert.Equal("Test Form", result[0].Name);
//    }

//    [Fact]
//    public async Task SearchFormData_ReturnsPagingResponse_WhenFormsFound()
//    {
//        // Arrange
//        var searchRequest = new FormSearchRequest();
//        var tenantId = 1;
//        var formData = new FormData
//        {
//            FormId = "test-form-id",
//            TenantId = tenantId,
//            DocumentId = "test-document-id",
//            Data = "{}"
//        };

//        var dbSetMock = new Mock<DbSet<FormData>>();
//        dbSetMock.As<IQueryable<FormData>>()
//            .Setup(m => m.Provider)
//            .Returns(new[] { formData }.AsQueryable().Provider);
//        dbSetMock.As<IQueryable<FormData>>()
//            .Setup(m => m.Expression)
//            .Returns(new[] { formData }.AsQueryable().Expression);
//        dbSetMock.As<IQueryable<FormData>>()
//            .Setup(m => m.ElementType)
//            .Returns(new[] { formData }.AsQueryable().ElementType);
//        dbSetMock.As<IQueryable<FormData>>()
//            .Setup(m => m.GetEnumerator())
//            .Returns(new[] { formData }.AsQueryable().GetEnumerator());

//        _dbContextMock.Setup(x => x.FormData)
//            .Returns(dbSetMock.Object);

//        // Act
//        var result = await _formRepository.SearchFormData(searchRequest, tenantId);

//        // Assert
//        Assert.NotNull(result);
//        Assert.Equal(1, result.TotalCount);
//        Assert.Single(result.Items);
//        Assert.Equal("test-form-id", result.Items[0].FormId);
//        Assert.Equal(tenantId, result.Items[0].TenantId);
//    }
//} 