//using Cpro.Forms.Data.Infrastructure;
//using Cpro.Forms.Data.Models;
//using Cpro.Forms.Data.Repositories;
//using Microsoft.EntityFrameworkCore;
//using Moq;
//using Peritos.Common.Abstractions;

//namespace Cpro.Forms.Data.Tests.Repositories;

//public class FormDesignerHistoryRepositoryTests
//{
//    private readonly Mock<SqlContext> _dbContextMock;
//    private readonly FormDesignHistoryRepository _repository;

//    public FormDesignerHistoryRepositoryTests()
//    {
//        var options = new DbContextOptionsBuilder<SqlContext>()
//            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
//            .Options;
//        var mockRequestContext = new Mock<IRequestContext>();
//        _dbContextMock = new Mock<SqlContext>(options, mockRequestContext.Object);
//        _repository = new FormDesignHistoryRepository(_dbContextMock.Object);
//    }

//    [Fact]
//    public async Task GetAllVersions_ReturnsFormDesignHistoryList_WhenVersionsExist()
//    {
//        // Arrange
//        var formId = "test-form-id";
//        var history = new FormDesignHistory
//        {
//            Id = "history-1",
//            FormDesignId = formId,
//            Version = 1
//        };

//        var data = new List<FormDesignHistory> { history }.AsQueryable();

//        var dbSetMock = new Mock<DbSet<FormDesignHistory>>();
//        dbSetMock.As<IQueryable<FormDesignHistory>>().Setup(m => m.Provider).Returns(data.Provider);
//        dbSetMock.As<IQueryable<FormDesignHistory>>().Setup(m => m.Expression).Returns(data.Expression);
//        dbSetMock.As<IQueryable<FormDesignHistory>>().Setup(m => m.ElementType).Returns(data.ElementType);
//        dbSetMock.As<IQueryable<FormDesignHistory>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

//        _dbContextMock.Setup(x => x.FormDesignsHistory).Returns(dbSetMock.Object);

//        // Act
//        var result = await _repository.GetAllVersions(formId);

//        // Assert
//        Assert.NotNull(result);
//        Assert.Single(result);
//        Assert.Equal(1, result[0].Version);
//        Assert.Equal(formId, result[0].FormDesignId);
//    }

//    [Fact]
//    public async Task GetVersion_ReturnsFormDesignHistory_WhenVersionExists()
//    {
//        // Arrange
//        var formId = "test-form-id";
//        var version = 1;
//        var history = new FormDesignHistory
//        {
//            Id = "history-1",
//            FormDesignId = formId,
//            Version = version
//        };

//        var data = new List<FormDesignHistory> { history }.AsQueryable();

//        var dbSetMock = new Mock<DbSet<FormDesignHistory>>();
//        dbSetMock.As<IQueryable<FormDesignHistory>>().Setup(m => m.Provider).Returns(data.Provider);
//        dbSetMock.As<IQueryable<FormDesignHistory>>().Setup(m => m.Expression).Returns(data.Expression);
//        dbSetMock.As<IQueryable<FormDesignHistory>>().Setup(m => m.ElementType).Returns(data.ElementType);
//        dbSetMock.As<IQueryable<FormDesignHistory>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

//        _dbContextMock.Setup(x => x.FormDesignsHistory).Returns(dbSetMock.Object);

//        // Act
//        var result = await _repository.GetVersion(formId, version);

//        // Assert
//        Assert.NotNull(result);
//        Assert.Equal(formId, result.FormDesignId);
//        Assert.Equal(version, result.Version);
//    }

//    [Fact]
//    public async Task SaveFormDesignHistoryAsync_AddsHistory_WhenCalled()
//    {
//        // Arrange
//        var history = new FormDesignHistory
//        {
//            Id = "history-1",
//            FormDesignId = "test-form-id",
//            Version = 1
//        };

//        var dbSetMock = new Mock<DbSet<FormDesignHistory>>();
//        _dbContextMock.Setup(x => x.FormDesignsHistory).Returns(dbSetMock.Object);

//        // Act
//        await _repository.SaveFormDesignHistoryAsync(history);

//        // Assert
//        dbSetMock.Verify(x => x.Add(It.Is<FormDesignHistory>(h => h == history)), Times.Once);
//        _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//    }

//    [Fact]
//    public async Task Delete_RemovesHistory_WhenCalled()
//    {
//        // Arrange
//        var history = new FormDesignHistory
//        {
//            Id = "history-1",
//            FormDesignId = "test-form-id",
//            Version = 1
//        };

//        var dbSetMock = new Mock<DbSet<FormDesignHistory>>();
//        _dbContextMock.Setup(x => x.FormDesignsHistory).Returns(dbSetMock.Object);

//        // Act
//        await _repository.Delete(history);

//        // Assert
//        dbSetMock.Verify(x => x.Remove(It.Is<FormDesignHistory>(h => h == history)), Times.Once);
//        _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//    }
//}