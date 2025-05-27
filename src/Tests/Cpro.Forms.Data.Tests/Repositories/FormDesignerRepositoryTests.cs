//using Cpro.Forms.Data.Infrastructure;
//using Cpro.Forms.Data.Models;
//using Cpro.Forms.Data.Repositories;
//using Microsoft.EntityFrameworkCore;
//using Moq;

//namespace Cpro.Forms.Data.Tests.Repositories;

//public class FormDesignerRepositoryTests
//{
//    private readonly Mock<SqlContext> _dbContextMock;
//    private readonly FormDesignRepository _formDesignerRepository;

//    public FormDesignerRepositoryTests()
//    {
//        var options = new DbContextOptionsBuilder<SqlContext>()
//            .UseInMemoryDatabase(databaseName: "TestDb")
//            .Options;
//        _dbContextMock = new Mock<SqlContext>(options);
//        _formDesignerRepository = new FormDesignerRepository(_dbContextMock.Object);
//    }

//    [Fact]
//    public async Task GetFormDefinitionResponseAsync_ReturnsDocumentResponse_WhenFormExists()
//    {
//        // Arrange
//        var formId = "test-form-id";
//        var tenantId = 1;
//        var formDesign = new FormDesign
//        {
//            FormId = formId,
//            TenantId = tenantId,
//            Data = "{}"
//        };

//        var dbSetMock = new Mock<DbSet<FormDesign>>();
//        dbSetMock.As<IQueryable<FormDesign>>()
//            .Setup(m => m.Provider)
//            .Returns(new[] { formDesign }.AsQueryable().Provider);
//        dbSetMock.As<IQueryable<FormDesign>>()
//            .Setup(m => m.Expression)
//            .Returns(new[] { formDesign }.AsQueryable().Expression);
//        dbSetMock.As<IQueryable<FormDesign>>()
//            .Setup(m => m.ElementType)
//            .Returns(new[] { formDesign }.AsQueryable().ElementType);
//        dbSetMock.As<IQueryable<FormDesign>>()
//            .Setup(m => m.GetEnumerator())
//            .Returns(new[] { formDesign }.AsQueryable().GetEnumerator());

//        _dbContextMock.Setup(x => x.FormDesign)
//            .Returns(dbSetMock.Object);

//        // Act
//        var result = await _formDesignerRepository.GetFormDefinitionResponseAsync(formId, tenantId);

//        // Assert
//        Assert.NotNull(result);
//        Assert.Equal(formId, result.FormId);
//        Assert.Equal(tenantId, result.TenantId);
//    }

//    [Fact]
//    public async Task CreateFormDefinitionAsync_ReturnsCreateFormDefinitionResponse_WhenCreationSuccessful()
//    {
//        // Arrange
//        var fieldRequest = new FieldRequest();
//        var formId = string.Empty;
//        var tenantId = 1;
//        var userEmail = "test@example.com";
//        var formDesign = new FormDesign
//        {
//            FormId = "new-form-id",
//            TenantId = tenantId,
//            Data = "{}",
//            CreatedBy = userEmail
//        };

//        var dbSetMock = new Mock<DbSet<FormDesign>>();
//        _dbContextMock.Setup(x => x.FormDesigns)
//            .Returns(dbSetMock.Object);

//        // Act
//        var result = await _formDesignerRepository.CreateFormDefinitionAsync(fieldRequest, formId, tenantId, userEmail);

//        // Assert
//        Assert.NotNull(result);
//        dbSetMock.Verify(x => x.Add(It.IsAny<FormDesign>()), Times.Once);
//        _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//    }

//    [Fact]
//    public async Task ActivateFormDefinitionAsync_CompletesSuccessfully_WhenActivationSuccessful()
//    {
//        // Arrange
//        var formId = "test-form-id";
//        var isActive = true;
//        var tenantId = 1;
//        var formDesign = new FormDesign
//        {
//            Id = formId,
//            TenantId = tenantId,
//            IsActive = isActive
//        };

//        var dbSetMock = new Mock<DbSet<FormDesign>>();
//        dbSetMock.As<IQueryable<FormDesign>>()
//            .Setup(m => m.Provider)
//            .Returns(new[] { formDesign }.AsQueryable().Provider);
//        dbSetMock.As<IQueryable<FormDesign>>()
//            .Setup(m => m.Expression)
//            .Returns(new[] { formDesign }.AsQueryable().Expression);
//        dbSetMock.As<IQueryable<FormDesign>>()
//            .Setup(m => m.ElementType)
//            .Returns(new[] { formDesign }.AsQueryable().ElementType);
//        dbSetMock.As<IQueryable<FormDesign>>()
//            .Setup(m => m.GetEnumerator())
//            .Returns(new[] { formDesign }.AsQueryable().GetEnumerator());

//        _dbContextMock.Setup(x => x.FormDesigns)
//            .Returns(dbSetMock.Object);

//        // Act
//        await _formDesignerRepository.ActivateFormDefinitionAsync(formId, isActive, tenantId);

//        // Assert
//        _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//    }

//    [Fact]
//    public async Task DeleteFormDesignAsync_CompletesSuccessfully_WhenDeletionSuccessful()
//    {
//        // Arrange
//        var formId = "test-form-id";
//        var tenantId = 1;
//        var formDesign = new FormDesign
//        {
//            FormId = formId,
//            TenantId = tenantId
//        };

//        var dbSetMock = new Mock<DbSet<FormDesign>>();
//        dbSetMock.As<IQueryable<FormDesign>>()
//            .Setup(m => m.Provider)
//            .Returns(new[] { formDesign }.AsQueryable().Provider);
//        dbSetMock.As<IQueryable<FormDesign>>()
//            .Setup(m => m.Expression)
//            .Returns(new[] { formDesign }.AsQueryable().Expression);
//        dbSetMock.As<IQueryable<FormDesign>>()
//            .Setup(m => m.ElementType)
//            .Returns(new[] { formDesign }.AsQueryable().ElementType);
//        dbSetMock.As<IQueryable<FormDesign>>()
//            .Setup(m => m.GetEnumerator())
//            .Returns(new[] { formDesign }.AsQueryable().GetEnumerator());

//        _dbContextMock.Setup(x => x.FormDesigns)
//            .Returns(dbSetMock.Object);

//        // Act
//        await _formDesignerRepository.DeleteFormDesignAsync(formId, tenantId);

//        // Assert
//        dbSetMock.Verify(x => x.Remove(It.IsAny<FormDesign>()), Times.Once);
//        _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//    }

//    [Fact]
//    public async Task SearchFormDesignsAsync_ReturnsPagingResponse_WhenFormsFound()
//    {
//        // Arrange
//        var searchRequest = new SearchRequest();
//        var tenantId = 1;
//        var formDesign = new FormDesign
//        {
//            FormId = "test-form-id",
//            TenantId = tenantId,
//            Data = "{}"
//        };

//        var dbSetMock = new Mock<DbSet<FormDesign>>();
//        dbSetMock.As<IQueryable<FormDesign>>()
//            .Setup(m => m.Provider)
//            .Returns(new[] { formDesign }.AsQueryable().Provider);
//        dbSetMock.As<IQueryable<FormDesign>>()
//            .Setup(m => m.Expression)
//            .Returns(new[] { formDesign }.AsQueryable().Expression);
//        dbSetMock.As<IQueryable<FormDesign>>()
//            .Setup(m => m.ElementType)
//            .Returns(new[] { formDesign }.AsQueryable().ElementType);
//        dbSetMock.As<IQueryable<FormDesign>>()
//            .Setup(m => m.GetEnumerator())
//            .Returns(new[] { formDesign }.AsQueryable().GetEnumerator());

//        _dbContextMock.Setup(x => x.FormDesigns)
//            .Returns(dbSetMock.Object);

//        // Act
//        var result = await _formDesignerRepository.SearchFormDesignsAsync(searchRequest, tenantId);

//        // Assert
//        Assert.NotNull(result);
//        Assert.Equal(1, result.TotalCount);
//        Assert.Single(result.Items);
//        Assert.Equal("test-form-id", result.Items[0].FormId);
//        Assert.Equal(tenantId, result.Items[0].TenantId);
//    }

//    [Fact]
//    public async Task DuplicateFormDefinitionAsync_ReturnsCreateFormDefinitionResponse_WhenDuplicationSuccessful()
//    {
//        // Arrange
//        var formId = "test-form-id";
//        var userEmail = "test@example.com";
//        var formDesign = new FormDesign
//        {
//            FormId = formId,
//            TenantId = 1,
//            Data = "{}",
//            CreatedBy = userEmail
//        };

//        var dbSetMock = new Mock<DbSet<FormDesign>>();
//        dbSetMock.As<IQueryable<FormDesign>>()
//            .Setup(m => m.Provider)
//            .Returns(new[] { formDesign }.AsQueryable().Provider);
//        dbSetMock.As<IQueryable<FormDesign>>()
//            .Setup(m => m.Expression)
//            .Returns(new[] { formDesign }.AsQueryable().Expression);
//        dbSetMock.As<IQueryable<FormDesign>>()
//            .Setup(m => m.ElementType)
//            .Returns(new[] { formDesign }.AsQueryable().ElementType);
//        dbSetMock.As<IQueryable<FormDesign>>()
//            .Setup(m => m.GetEnumerator())
//            .Returns(new[] { formDesign }.AsQueryable().GetEnumerator());

//        _dbContextMock.Setup(x => x.FormDesigns)
//            .Returns(dbSetMock.Object);

//        // Act
//        var result = await _formDesignerRepository.DuplicateFormDefinitionAsync(formId, userEmail);

//        // Assert
//        Assert.NotNull(result);
//        dbSetMock.Verify(x => x.Add(It.IsAny<FormDesign>()), Times.Once);
//        _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//    }

//    [Fact]
//    public async Task GetFormDataJsonAsync_ReturnsJsonString_WhenFormExists()
//    {
//        // Arrange
//        var formId = "test-form-id";
//        var expectedJson = "{\"form\": \"data\"}";
//        var formDesign = new FormDesign
//        {
//            FormId = formId,
//            Data = expectedJson
//        };

//        var dbSetMock = new Mock<DbSet<FormDesign>>();
//        dbSetMock.As<IQueryable<FormDesign>>()
//            .Setup(m => m.Provider)
//            .Returns(new[] { formDesign }.AsQueryable().Provider);
//        dbSetMock.As<IQueryable<FormDesign>>()
//            .Setup(m => m.Expression)
//            .Returns(new[] { formDesign }.AsQueryable().Expression);
//        dbSetMock.As<IQueryable<FormDesign>>()
//            .Setup(m => m.ElementType)
//            .Returns(new[] { formDesign }.AsQueryable().ElementType);
//        dbSetMock.As<IQueryable<FormDesign>>()
//            .Setup(m => m.GetEnumerator())
//            .Returns(new[] { formDesign }.AsQueryable().GetEnumerator());

//        _dbContextMock.Setup(x => x.FormDesigns)
//            .Returns(dbSetMock.Object);

//        // Act
//        var result = await _formDesignerRepository.GetFormDataJsonAsync(formId);

//        // Assert
//        Assert.Equal(expectedJson, result);
//    }
//} 