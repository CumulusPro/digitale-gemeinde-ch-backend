using Cpro.Forms.Integration.Storage.Configuration;
using Cpro.Forms.Integration.Storage.Services;
using Moq;
using Xunit;

namespace Cpro.Forms.Integration.Storage.Tests;

public class AzureBlobServiceTests
{
    private readonly Mock<IStorageConfiguration> _configMock;
    private readonly AzureBlobService _service;

    public AzureBlobServiceTests()
    {
        _configMock = new Mock<IStorageConfiguration>();
        _configMock.Setup(x => x.AzureStorageConnectionString).Returns("UseDevelopmentStorage=true");
        _configMock.Setup(x => x.ContainerName).Returns("test-container");

        _service = new AzureBlobService(_configMock.Object);
    }

    [Fact]
    public void GetBlobClient_ReturnsCorrectBlobClient()
    {
        // Arrange
        var fileName = "test-file.json";

        // Act
        var blobClient = _service.GetBlobClient(fileName);

        // Assert
        Assert.NotNull(blobClient);
        Assert.Equal(fileName, blobClient.Name);
    }

    [Fact]
    public void GetSignedUrl_ReturnsValidUrl()
    {
        // Arrange
        var fileName = "test-file.json";

        // Act
        var signedUrl = _service.GetSignedUrl(fileName);

        // Assert
        Assert.NotNull(signedUrl);
        Assert.Contains(fileName, signedUrl);
    }
}
