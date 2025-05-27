//using Cpro.Forms.Data.Infrastructure;
//using Cpro.Forms.Data.Models;
//using Cpro.Forms.Data.Repositories;
//using Microsoft.EntityFrameworkCore;
//using Moq;
//using Xunit;

//namespace Cpro.Forms.Data.Tests.Repositories;

//public class StorageRepositoryTests
//{
//    private readonly Mock<FormsDbContext> _dbContextMock;
//    private readonly StorageRepository _storageRepository;

//    public StorageRepositoryTests()
//    {
//        var options = new DbContextOptionsBuilder<FormsDbContext>()
//            .UseInMemoryDatabase(databaseName: "TestDb")
//            .Options;
//        _dbContextMock = new Mock<FormsDbContext>(options);
//        _storageRepository = new StorageRepository(_dbContextMock.Object);
//    }

//    [Fact]
//    public async Task GetStorageFile_ReturnsFile_WhenFileExists()
//    {
//        // Arrange
//        var fileId = "test-file-id";
//        var storageFile = new StorageFile
//        {
//            FileId = fileId,
//            FileName = "test.txt",
//            ContentType = "text/plain",
//            FileData = new byte[] { 1, 2, 3 }
//        };

//        var dbSetMock = new Mock<DbSet<StorageFile>>();
//        dbSetMock.As<IQueryable<StorageFile>>()
//            .Setup(m => m.Provider)
//            .Returns(new[] { storageFile }.AsQueryable().Provider);
//        dbSetMock.As<IQueryable<StorageFile>>()
//            .Setup(m => m.Expression)
//            .Returns(new[] { storageFile }.AsQueryable().Expression);
//        dbSetMock.As<IQueryable<StorageFile>>()
//            .Setup(m => m.ElementType)
//            .Returns(new[] { storageFile }.AsQueryable().ElementType);
//        dbSetMock.As<IQueryable<StorageFile>>()
//            .Setup(m => m.GetEnumerator())
//            .Returns(new[] { storageFile }.AsQueryable().GetEnumerator());

//        _dbContextMock.Setup(x => x.StorageFile)
//            .Returns(dbSetMock.Object);

//        // Act
//        var result = await _storageRepository.GetStorageFile(fileId);

//        // Assert
//        Assert.NotNull(result);
//        Assert.Equal(fileId, result.FileId);
//        Assert.Equal("test.txt", result.FileName);
//        Assert.Equal("text/plain", result.ContentType);
//        Assert.Equal(new byte[] { 1, 2, 3 }, result.FileData);
//    }

//    [Fact]
//    public async Task CreateStorageFile_ReturnsFile_WhenCreationSuccessful()
//    {
//        // Arrange
//        var file = new StorageFile
//        {
//            FileId = "new-file-id",
//            FileName = "new.txt",
//            ContentType = "text/plain",
//            FileData = new byte[] { 4, 5, 6 }
//        };

//        var dbSetMock = new Mock<DbSet<StorageFile>>();
//        _dbContextMock.Setup(x => x.StorageFile)
//            .Returns(dbSetMock.Object);

//        // Act
//        var result = await _storageRepository.CreateStorageFile(file);

//        // Assert
//        Assert.NotNull(result);
//        dbSetMock.Verify(x => x.Add(It.IsAny<StorageFile>()), Times.Once);
//        _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//    }

//    [Fact]
//    public async Task UpdateStorageFile_ReturnsFile_WhenUpdateSuccessful()
//    {
//        // Arrange
//        var fileId = "test-file-id";
//        var file = new StorageFile
//        {
//            FileId = fileId,
//            FileName = "updated.txt",
//            ContentType = "text/plain",
//            FileData = new byte[] { 7, 8, 9 }
//        };

//        var dbSetMock = new Mock<DbSet<StorageFile>>();
//        dbSetMock.As<IQueryable<StorageFile>>()
//            .Setup(m => m.Provider)
//            .Returns(new[] { file }.AsQueryable().Provider);
//        dbSetMock.As<IQueryable<StorageFile>>()
//            .Setup(m => m.Expression)
//            .Returns(new[] { file }.AsQueryable().Expression);
//        dbSetMock.As<IQueryable<StorageFile>>()
//            .Setup(m => m.ElementType)
//            .Returns(new[] { file }.AsQueryable().ElementType);
//        dbSetMock.As<IQueryable<StorageFile>>()
//            .Setup(m => m.GetEnumerator())
//            .Returns(new[] { file }.AsQueryable().GetEnumerator());

//        _dbContextMock.Setup(x => x.StorageFile)
//            .Returns(dbSetMock.Object);

//        // Act
//        var result = await _storageRepository.UpdateStorageFile(fileId, file);

//        // Assert
//        Assert.NotNull(result);
//        _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//    }

//    [Fact]
//    public async Task DeleteStorageFile_CompletesSuccessfully_WhenDeletionSuccessful()
//    {
//        // Arrange
//        var fileId = "test-file-id";
//        var file = new StorageFile
//        {
//            FileId = fileId
//        };

//        var dbSetMock = new Mock<DbSet<StorageFile>>();
//        dbSetMock.As<IQueryable<StorageFile>>()
//            .Setup(m => m.Provider)
//            .Returns(new[] { file }.AsQueryable().Provider);
//        dbSetMock.As<IQueryable<StorageFile>>()
//            .Setup(m => m.Expression)
//            .Returns(new[] { file }.AsQueryable().Expression);
//        dbSetMock.As<IQueryable<StorageFile>>()
//            .Setup(m => m.ElementType)
//            .Returns(new[] { file }.AsQueryable().ElementType);
//        dbSetMock.As<IQueryable<StorageFile>>()
//            .Setup(m => m.GetEnumerator())
//            .Returns(new[] { file }.AsQueryable().GetEnumerator());

//        _dbContextMock.Setup(x => x.StorageFile)
//            .Returns(dbSetMock.Object);

//        // Act
//        await _storageRepository.DeleteStorageFile(fileId);

//        // Assert
//        dbSetMock.Verify(x => x.Remove(It.IsAny<StorageFile>()), Times.Once);
//        _dbContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//    }

//    [Fact]
//    public async Task GetStorageFilesByFormId_ReturnsFileList_WhenFilesExist()
//    {
//        // Arrange
//        var formId = "test-form-id";
//        var storageFile = new StorageFile
//        {
//            FileId = "test-file-id",
//            FormId = formId,
//            FileName = "test.txt"
//        };

//        var dbSetMock = new Mock<DbSet<StorageFile>>();
//        dbSetMock.As<IQueryable<StorageFile>>()
//            .Setup(m => m.Provider)
//            .Returns(new[] { storageFile }.AsQueryable().Provider);
//        dbSetMock.As<IQueryable<StorageFile>>()
//            .Setup(m => m.Expression)
//            .Returns(new[] { storageFile }.AsQueryable().Expression);
//        dbSetMock.As<IQueryable<StorageFile>>()
//            .Setup(m => m.ElementType)
//            .Returns(new[] { storageFile }.AsQueryable().ElementType);
//        dbSetMock.As<IQueryable<StorageFile>>()
//            .Setup(m => m.GetEnumerator())
//            .Returns(new[] { storageFile }.AsQueryable().GetEnumerator());

//        _dbContextMock.Setup(x => x.StorageFile)
//            .Returns(dbSetMock.Object);

//        // Act
//        var result = await _storageRepository.GetStorageFilesByFormId(formId);

//        // Assert
//        Assert.NotNull(result);
//        Assert.Single(result);
//        Assert.Equal(formId, result[0].FormId);
//        Assert.Equal("test.txt", result[0].FileName);
//    }
//} 