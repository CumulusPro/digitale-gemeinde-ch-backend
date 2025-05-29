using Cpro.Forms.Integration.Storage.Configuration;
using Cpro.Forms.Integration.Storage.Services;
using Moq;
using System.Text;
using Xunit;

namespace Cpro.Forms.Integration.Storage.Tests;

public class OnPremStorageServiceTests
{
    private readonly Mock<IStorageConfiguration> _configMock;
    private readonly OnPremStorageService _service;
    private readonly string _testDirectory;

    public OnPremStorageServiceTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDirectory);

        _configMock = new Mock<IStorageConfiguration>();
        _configMock.Setup(x => x.OnPremStoragePath).Returns(_testDirectory);

        _service = new OnPremStorageService(_configMock.Object);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, recursive: true);
        }
    }

    [Fact]
    public async Task GetFile_ReturnsFileContent_WhenFileExists()
    {
        var fileName = "test-file.json";
        var expectedContent = "{\"key\":\"value\"}";
        File.WriteAllText(Path.Combine(_testDirectory, fileName), expectedContent);

        var content = await _service.GetFile(fileName);

        Assert.NotNull(content);
        Assert.Equal(expectedContent, content);
    }

    [Fact]
    public async Task GetFile_ReturnsNull_WhenFileDoesNotExist()
    {
        var content = await _service.GetFile("non-existent.json");

        Assert.Null(content);
    }

    [Fact]
    public void GetSignedUrl_ReturnsPath_IfFileExists()
    {
        var fileName = "file.txt";
        var filePath = Path.Combine(_testDirectory, fileName);
        File.WriteAllText(filePath, "test");

        var signedUrl = _service.GetSignedUrl(fileName);

        Assert.Contains(fileName, signedUrl);
    }

    [Fact]
    public async Task UploadFile_WithStream_WritesFile()
    {
        var fileName = "upload-file.json";
        var content = "{\"test\":true}";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        await _service.UploadFile(fileName, stream);

        var fullPath = Path.Combine(_testDirectory, fileName);
        Assert.True(File.Exists(fullPath));
    }

    [Fact]
    public async Task DeleteFile_RemovesFile()
    {
        var fileName = "delete-me.txt";
        var path = Path.Combine(_testDirectory, fileName);
        File.WriteAllText(path, "test");

        await _service.DeleteFile(fileName);

        Assert.False(File.Exists(path));
    }

    [Fact]
    public async Task DeleteFolder_RemovesAllFilesInFolder()
    {
        var folderName = "to-delete";
        var folderPath = Path.Combine(_testDirectory, folderName);
        Directory.CreateDirectory(folderPath);
        File.WriteAllText(Path.Combine(folderPath, "file1.txt"), "1");
        File.WriteAllText(Path.Combine(folderPath, "file2.txt"), "2");

        await _service.DeleteFolder(folderName);

        Assert.False(Directory.Exists(folderPath));
    }
}