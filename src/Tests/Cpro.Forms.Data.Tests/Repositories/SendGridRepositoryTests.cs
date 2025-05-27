//using Cpro.Forms.Data.Infrastructure;
//using Cpro.Forms.Data.Models;
//using Cpro.Forms.Data.Repositories;
//using Microsoft.EntityFrameworkCore;
//using Moq;
//using Xunit;

//namespace Cpro.Forms.Data.Tests.Repositories;

//public class SendGridRepositoryTests
//{
//    private readonly Mock<FormsDbContext> _dbContextMock;
//    private readonly SendGridRepository _sendGridRepository;

//    public SendGridRepositoryTests()
//    {
//        var options = new DbContextOptionsBuilder<FormsDbContext>()
//            .UseInMemoryDatabase(databaseName: "TestDb")
//            .Options;
//        _dbContextMock = new Mock<FormsDbContext>(options);
//        _sendGridRepository = new SendGridRepository(_dbContextMock.Object);
//    }

//    [Fact]
//    public async Task GetSendGridTemplate_ReturnsTemplate_WhenTemplateExists()
//    {
//        // Arrange
//        var templateId = "test-template-id";
//        var sendGridTemplate = new SendGridTemplate
//        {
//            TemplateId = templateId,
//            Content = "Test content"
//        };

//        var dbSetMock = new Mock<DbSet<SendGridTemplate>>();
//        dbSetMock.As<IQueryable<SendGridTemplate>>()
//            .Setup(m => m.Provider)
//            .Returns(new[] { sendGridTemplate }.AsQueryable().Provider);
//        dbSetMock.As<IQueryable<SendGridTemplate>>()
//            .Setup(m => m.Expression)
//            .Returns(new[] { sendGridTemplate }.AsQueryable().Expression);
//        dbSetMock.As<IQueryable<SendGridTemplate>>()
//            .Setup(m => m.ElementType)
//            .Returns(new[] { sendGridTemplate }.AsQueryable().ElementType);
//        dbSetMock.As<IQueryable<SendGridTemplate>>()
//            .Setup(m => m.GetEnumerator())
//            .Returns(new[] { sendGridTemplate }.AsQueryable().GetEnumerator());

//        _dbContextMock.Setup(x => x.SendGridTemplate)
//            .Returns(dbSetMock.Object);

//        // Act
//        var result = await _sendGridRepository.GetSendGridTemplate(templateId);

//        // Assert
//        Assert.NotNull(result);
//        Assert.Equal(templateId, result.TemplateId);
//        Assert.Equal("Test content", result.Content);
//    }

//    [Fact]
//    public async Task CreateSendGridTemplate_ReturnsTemplate_WhenCreationSuccessful()
//    {
//        // Arrange
//        var template = new SendGridTemplate
//        {
//            TemplateId = "new-template-id",
//            Content = "New content"
//        };

//        var dbSetMock = new Mock<DbSet<SendGridTemplate>>();
//        _dbContextMock.Setup(x => x.SendGridTemplate)
//            .Returns(dbSetMock.Object);

//        // Act
//        var result = await _sendGridRepository.CreateSendGridTemplate(template);

//        // Assert
//        Assert.NotNull(result);
//        dbSetMock.Verify(x => x.Add(It.IsAny<SendGridTemplate>()), Times.Once);
//        _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//    }

//    [Fact]
//    public async Task UpdateSendGridTemplate_ReturnsTemplate_WhenUpdateSuccessful()
//    {
//        // Arrange
//        var templateId = "test-template-id";
//        var template = new SendGridTemplate
//        {
//            TemplateId = templateId,
//            Content = "Updated content"
//        };

//        var dbSetMock = new Mock<DbSet<SendGridTemplate>>();
//        dbSetMock.As<IQueryable<SendGridTemplate>>()
//            .Setup(m => m.Provider)
//            .Returns(new[] { template }.AsQueryable().Provider);
//        dbSetMock.As<IQueryable<SendGridTemplate>>()
//            .Setup(m => m.Expression)
//            .Returns(new[] { template }.AsQueryable().Expression);
//        dbSetMock.As<IQueryable<SendGridTemplate>>()
//            .Setup(m => m.ElementType)
//            .Returns(new[] { template }.AsQueryable().ElementType);
//        dbSetMock.As<IQueryable<SendGridTemplate>>()
//            .Setup(m => m.GetEnumerator())
//            .Returns(new[] { template }.AsQueryable().GetEnumerator());

//        _dbContextMock.Setup(x => x.SendGridTemplate)
//            .Returns(dbSetMock.Object);

//        // Act
//        var result = await _sendGridRepository.UpdateSendGridTemplate(templateId, template);

//        // Assert
//        Assert.NotNull(result);
//        _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//    }

//    [Fact]
//    public async Task DeleteSendGridTemplate_CompletesSuccessfully_WhenDeletionSuccessful()
//    {
//        // Arrange
//        var templateId = "test-template-id";
//        var template = new SendGridTemplate
//        {
//            TemplateId = templateId
//        };

//        var dbSetMock = new Mock<DbSet<SendGridTemplate>>();
//        dbSetMock.As<IQueryable<SendGridTemplate>>()
//            .Setup(m => m.Provider)
//            .Returns(new[] { template }.AsQueryable().Provider);
//        dbSetMock.As<IQueryable<SendGridTemplate>>()
//            .Setup(m => m.Expression)
//            .Returns(new[] { template }.AsQueryable().Expression);
//        dbSetMock.As<IQueryable<SendGridTemplate>>()
//            .Setup(m => m.ElementType)
//            .Returns(new[] { template }.AsQueryable().ElementType);
//        dbSetMock.As<IQueryable<SendGridTemplate>>()
//            .Setup(m => m.GetEnumerator())
//            .Returns(new[] { template }.AsQueryable().GetEnumerator());

//        _dbContextMock.Setup(x => x.SendGridTemplate)
//            .Returns(dbSetMock.Object);

//        // Act
//        await _sendGridRepository.DeleteSendGridTemplate(templateId);

//        // Assert
//        dbSetMock.Verify(x => x.Remove(It.IsAny<SendGridTemplate>()), Times.Once);
//        _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//    }
//} 