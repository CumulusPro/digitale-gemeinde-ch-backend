//using Cpro.Forms.Data.Models;
//using Cpro.Forms.Data.Repositories;
//using Microsoft.EntityFrameworkCore;
//using Moq;

//namespace Cpro.Forms.Data.Tests.Repositories;

//public class FormTemplateRepositoryTests
//{
//    private readonly Mock<FormsDbContext> _dbContextMock;
//    private readonly FormTemplateRepository _formTemplateRepository;

//    public FormTemplateRepositoryTests()
//    {
//        var options = new DbContextOptionsBuilder<FormsDbContext>()
//            .UseInMemoryDatabase(databaseName: "TestDb")
//            .Options;
//        _dbContextMock = new Mock<FormsDbContext>(options);
//        _formTemplateRepository = new FormTemplateRepository(_dbContextMock.Object);
//    }

//    [Fact]
//    public async Task GetFormTemplate_ReturnsFormTemplate_WhenTemplateExists()
//    {
//        // Arrange
//        var templateId = "test-template-id";
//        var formTemplate = new FormTemplate
//        {
//            TemplateId = templateId,
//            Data = "{}"
//        };

//        var dbSetMock = new Mock<DbSet<FormTemplate>>();
//        dbSetMock.As<IQueryable<FormTemplate>>()
//            .Setup(m => m.Provider)
//            .Returns(new[] { formTemplate }.AsQueryable().Provider);
//        dbSetMock.As<IQueryable<FormTemplate>>()
//            .Setup(m => m.Expression)
//            .Returns(new[] { formTemplate }.AsQueryable().Expression);
//        dbSetMock.As<IQueryable<FormTemplate>>()
//            .Setup(m => m.ElementType)
//            .Returns(new[] { formTemplate }.AsQueryable().ElementType);
//        dbSetMock.As<IQueryable<FormTemplate>>()
//            .Setup(m => m.GetEnumerator())
//            .Returns(new[] { formTemplate }.AsQueryable().GetEnumerator());

//        _dbContextMock.Setup(x => x.FormTemplate)
//            .Returns(dbSetMock.Object);

//        // Act
//        var result = await _formTemplateRepository.GetFormTemplate(templateId);

//        // Assert
//        Assert.NotNull(result);
//        Assert.Equal(templateId, result.TemplateId);
//    }

//    [Fact]
//    public async Task CreateFormTemplate_ReturnsFormTemplate_WhenCreationSuccessful()
//    {
//        // Arrange
//        var templateRequest = new CreateFormTemplateRequest();
//        var tenantId = 1;
//        var formTemplate = new FormTemplate
//        {
//            TemplateId = "new-template-id",
//            TenantId = tenantId,
//            Data = "{}"
//        };

//        var dbSetMock = new Mock<DbSet<FormTemplate>>();
//        _dbContextMock.Setup(x => x.FormTemplate)
//            .Returns(dbSetMock.Object);

//        // Act
//        var result = await _formTemplateRepository.CreateFormTemplate(templateRequest, tenantId);

//        // Assert
//        Assert.NotNull(result);
//        dbSetMock.Verify(x => x.Add(It.IsAny<FormTemplate>()), Times.Once);
//        _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//    }

//    [Fact]
//    public async Task UpdateFormTemplate_ReturnsFormTemplate_WhenUpdateSuccessful()
//    {
//        // Arrange
//        var templateId = "test-template-id";
//        var template = new FormTemplate
//        {
//            TemplateId = templateId,
//            Data = "{}"
//        };
//        var tenantId = 1;

//        var dbSetMock = new Mock<DbSet<FormTemplate>>();
//        dbSetMock.As<IQueryable<FormTemplate>>()
//            .Setup(m => m.Provider)
//            .Returns(new[] { template }.AsQueryable().Provider);
//        dbSetMock.As<IQueryable<FormTemplate>>()
//            .Setup(m => m.Expression)
//            .Returns(new[] { template }.AsQueryable().Expression);
//        dbSetMock.As<IQueryable<FormTemplate>>()
//            .Setup(m => m.ElementType)
//            .Returns(new[] { template }.AsQueryable().ElementType);
//        dbSetMock.As<IQueryable<FormTemplate>>()
//            .Setup(m => m.GetEnumerator())
//            .Returns(new[] { template }.AsQueryable().GetEnumerator());

//        _dbContextMock.Setup(x => x.FormTemplate)
//            .Returns(dbSetMock.Object);

//        // Act
//        var result = await _formTemplateRepository.UpdateFormTemplate(templateId, template, tenantId);

//        // Assert
//        Assert.NotNull(result);
//        _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//    }

//    [Fact]
//    public async Task DeleteFormTemplate_CompletesSuccessfully_WhenDeletionSuccessful()
//    {
//        // Arrange
//        var templateId = "test-template-id";
//        var formTemplate = new FormTemplate
//        {
//            TemplateId = templateId
//        };

//        var dbSetMock = new Mock<DbSet<FormTemplate>>();
//        dbSetMock.As<IQueryable<FormTemplate>>()
//            .Setup(m => m.Provider)
//            .Returns(new[] { formTemplate }.AsQueryable().Provider);
//        dbSetMock.As<IQueryable<FormTemplate>>()
//            .Setup(m => m.Expression)
//            .Returns(new[] { formTemplate }.AsQueryable().Expression);
//        dbSetMock.As<IQueryable<FormTemplate>>()
//            .Setup(m => m.ElementType)
//            .Returns(new[] { formTemplate }.AsQueryable().ElementType);
//        dbSetMock.As<IQueryable<FormTemplate>>()
//            .Setup(m => m.GetEnumerator())
//            .Returns(new[] { formTemplate }.AsQueryable().GetEnumerator());

//        _dbContextMock.Setup(x => x.FormTemplate)
//            .Returns(dbSetMock.Object);

//        // Act
//        await _formTemplateRepository.DeleteFormTemplate(templateId);

//        // Assert
//        dbSetMock.Verify(x => x.Remove(It.IsAny<FormTemplate>()), Times.Once);
//        _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//    }

//    [Fact]
//    public async Task SearchFormTemplate_ReturnsPagingResponse_WhenTemplatesFound()
//    {
//        // Arrange
//        var searchRequest = new SearchRequest();
//        var formTemplate = new FormTemplate
//        {
//            TemplateId = "test-template-id",
//            Data = "{}"
//        };

//        var dbSetMock = new Mock<DbSet<FormTemplate>>();
//        dbSetMock.As<IQueryable<FormTemplate>>()
//            .Setup(m => m.Provider)
//            .Returns(new[] { formTemplate }.AsQueryable().Provider);
//        dbSetMock.As<IQueryable<FormTemplate>>()
//            .Setup(m => m.Expression)
//            .Returns(new[] { formTemplate }.AsQueryable().Expression);
//        dbSetMock.As<IQueryable<FormTemplate>>()
//            .Setup(m => m.ElementType)
//            .Returns(new[] { formTemplate }.AsQueryable().ElementType);
//        dbSetMock.As<IQueryable<FormTemplate>>()
//            .Setup(m => m.GetEnumerator())
//            .Returns(new[] { formTemplate }.AsQueryable().GetEnumerator());

//        _dbContextMock.Setup(x => x.FormTemplate)
//            .Returns(dbSetMock.Object);

//        // Act
//        var result = await _formTemplateRepository.SearchFormTemplate(searchRequest);

//        // Assert
//        Assert.NotNull(result);
//        Assert.Equal(1, result.TotalCount);
//        Assert.Single(result.Items);
//        Assert.Equal("test-template-id", result.Items[0].TemplateId);
//    }
//} 