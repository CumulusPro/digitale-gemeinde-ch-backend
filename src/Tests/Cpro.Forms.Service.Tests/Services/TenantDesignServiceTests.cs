using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Cpro.Forms.Integration.Storage.Services;
using Cpro.Forms.Service.Models;
using Cpro.Forms.Service.Services;
using Newtonsoft.Json;

namespace Cpro.Forms.Service.Tests.Services;

public class TenantDesignServiceTests
{
    private readonly Mock<IAzureBlobService> _azureBlobServiceMock;
    private readonly TenantDesignService _tenantDesignService;

    public TenantDesignServiceTests()
    {
        _azureBlobServiceMock = new Mock<IAzureBlobService>();
        _tenantDesignService = new TenantDesignService(_azureBlobServiceMock.Object);
    }

    [Fact]
    public async Task CreateUpdateTenantDesign_SavesDesign_WhenValidInput()
    {
        // Arrange
        var design = new TenantDesign
        {
            header = new Header { content = "Test Header" },
            footer = new Footer { content = "Test Footer" },
            designConfig = new DesignConfig()
        };
        var tenantId = 1;
        var expectedJson = JsonConvert.SerializeObject(design);

        MemoryStream? capturedStream = null;
        _azureBlobServiceMock
            .Setup(x => x.UploadFile($"{tenantId}.json", It.IsAny<MemoryStream>()))
            .Callback<string, MemoryStream>((_, ms) =>
            {
                capturedStream = new MemoryStream();
                ms.Position = 0;
                ms.CopyTo(capturedStream);
                capturedStream.Position = 0;
            })
            .Returns(Task.CompletedTask);

        // Act
        var result = await _tenantDesignService.CreateUpdateTenantDesign(design, tenantId);

        // Assert
        Assert.Equal(design.header?.content, result.header?.content);
        Assert.Equal(design.footer?.content, result.footer?.content);
        Assert.NotNull(capturedStream);
        using var reader = new StreamReader(capturedStream!);
        var content = reader.ReadToEnd();
        Assert.Equal(expectedJson, content);
        _azureBlobServiceMock.Verify(x => x.UploadFile($"{tenantId}.json", It.IsAny<MemoryStream>()), Times.Once);
    }

    [Fact]
    public async Task GetTenantDesign_ReturnsDesign_WhenDesignExists()
    {
        // Arrange
        var tenantId = 1;
        var expectedDesign = new TenantDesign
        {
            header = new Header { content = "Test Header" },
            footer = new Footer { content = "Test Footer" },
            designConfig = new DesignConfig()
        };

        var json = JsonConvert.SerializeObject(expectedDesign);
        var binaryData = BinaryData.FromString(json);

        var blobDownloadResult = BlobsModelFactory.BlobDownloadResult(content: binaryData);

        var existsResponseMock = new Mock<Response<bool>>();
        existsResponseMock.Setup(x => x.Value).Returns(true);

        var downloadResponseMock = new Mock<Response<BlobDownloadResult>>();
        downloadResponseMock.Setup(x => x.Value).Returns(blobDownloadResult);

        var blobClientMock = new Mock<BlobClient>();

        _azureBlobServiceMock
            .Setup(x => x.GetBlobClient($"{tenantId}.json"))
            .Returns(blobClientMock.Object);

        blobClientMock
            .Setup(x => x.ExistsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(existsResponseMock.Object);

        blobClientMock
            .Setup(x => x.DownloadContentAsync())
            .ReturnsAsync(downloadResponseMock.Object);

        // Act
        var result = await _tenantDesignService.GetTenantDesign(tenantId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedDesign.header?.content, result.header?.content);
        Assert.Equal(expectedDesign.footer?.content, result.footer?.content);
    }

    [Fact]
    public async Task GetTenantDesign_ReturnsNewDesign_WhenDesignDoesNotExist()
    {
        // Arrange
        var tenantId = 1;
        var blobClientMock = new Mock<BlobClient>();

        _azureBlobServiceMock.Setup(x => x.GetBlobClient($"{tenantId}.json"))
            .Returns(blobClientMock.Object);
        blobClientMock.Setup(x => x.ExistsAsync(default))
            .ReturnsAsync(Response.FromValue(false, null!));

        // Act
        var result = await _tenantDesignService.GetTenantDesign(tenantId);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.header);
        Assert.Null(result.footer);
        Assert.Null(result.designConfig);
        _azureBlobServiceMock.Verify(x => x.GetBlobClient($"{tenantId}.json"), Times.Once);
        blobClientMock.Verify(x => x.ExistsAsync(default), Times.Once);
        blobClientMock.Verify(x => x.DownloadContentAsync(default), Times.Never);
    }
}