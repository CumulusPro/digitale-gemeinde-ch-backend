using Cpro.Forms.Data.Repositories;
using Cpro.Forms.Integration.Storage.Services;
using Cpro.Forms.Service.Models;
using Cpro.Forms.Service.Services;
using Newtonsoft.Json;

namespace Cpro.Forms.Service.Tests.Services;

public class FormDesignerHistoryServiceTests
{
    private readonly Mock<IFormDesignHistoryRepository> _formDesignerHistoryRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IAzureBlobService> _azureBlobServiceMock;
    private readonly FormDesignerHistoryService _formDesignerHistoryService;

    public FormDesignerHistoryServiceTests()
    {
        _formDesignerHistoryRepositoryMock = new Mock<IFormDesignHistoryRepository>();
        _mapperMock = new Mock<IMapper>();
        _azureBlobServiceMock = new Mock<IAzureBlobService>();
        
        _formDesignerHistoryService = new FormDesignerHistoryService(
            _formDesignerHistoryRepositoryMock.Object,
            _mapperMock.Object,
            _azureBlobServiceMock.Object);
    }

    [Fact]
    public async Task GetAllVersions_ReturnsFormDesignList_WhenVersionsExist()
    {
        // Arrange
        var formId = "test-form-id";
        var expectedVersions = new List<Data.Models.FormDesignHistory>
        {
            new() { StorageUrl = "test-url-1" },
            new() { StorageUrl = "test-url-2" }
        };
        var expectedMappedVersions = new List<FormDesign>
        {
            new() { StorageUrl = "signed-url-1" },
            new() { StorageUrl = "signed-url-2" }
        };

        _formDesignerHistoryRepositoryMock.Setup(x => x.GetAllVersions(formId))
            .ReturnsAsync(expectedVersions);

        _azureBlobServiceMock.Setup(x => x.GetSignedUrl("test-url-1"))
            .Returns("signed-url-1");
        _azureBlobServiceMock.Setup(x => x.GetSignedUrl("test-url-2"))
            .Returns("signed-url-2");

        _mapperMock.Setup(x => x.Map<List<FormDesign>>(expectedVersions))
            .Returns(expectedMappedVersions);

        // Act
        var result = await _formDesignerHistoryService.GetAllVersions(formId);

        // Assert
        Assert.Equal(expectedMappedVersions, result);
        _formDesignerHistoryRepositoryMock.Verify(x => x.GetAllVersions(formId), Times.Once);
        _azureBlobServiceMock.Verify(x => x.GetSignedUrl(It.IsAny<string>()), Times.Exactly(2));
        _mapperMock.Verify(x => x.Map<List<FormDesign>>(expectedVersions), Times.Once);
    }

    [Fact]
    public async Task GetVersion_ReturnsFieldRequest_WhenVersionExists()
    {
        // Arrange
        var formId = "test-form-id";
        var version = 1;
        var expectedHistory = new Data.Models.FormDesignHistory { StorageUrl = "test-url" };
        var expectedJson = JsonConvert.SerializeObject(new FieldRequest());
        var expectedFieldRequest = JsonConvert.DeserializeObject<FieldRequest>(expectedJson);

        _formDesignerHistoryRepositoryMock.Setup(x => x.GetVersion(formId, version))
            .ReturnsAsync(expectedHistory);

        _azureBlobServiceMock.Setup(x => x.GetFile(expectedHistory.StorageUrl))
            .ReturnsAsync(expectedJson);

        // Act
        var result = await _formDesignerHistoryService.GetVersion(formId, version);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedFieldRequest.Id, result.Id);
        _formDesignerHistoryRepositoryMock.Verify(x => x.GetVersion(formId, version), Times.Once);
        _azureBlobServiceMock.Verify(x => x.GetFile(expectedHistory.StorageUrl), Times.Once);
    }

    [Fact]
    public async Task GetVersion_ThrowsFileNotFoundException_WhenVersionDoesNotExist()
    {
        // Arrange
        var formId = "test-form-id";
        var version = 1;

        _formDesignerHistoryRepositoryMock.Setup(x => x.GetVersion(formId, version))
            .ReturnsAsync((Data.Models.FormDesignHistory)null);

        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(() => 
            _formDesignerHistoryService.GetVersion(formId, version));
    }

    [Fact]
    public async Task SaveFormDesignVersionHistory_SavesHistory_WhenFormDesignExists()
    {
        // Arrange
        var formDesign = new Data.Models.FormDesign
        {
            Id = "test-id",
            Name = "Test Form",
            TenantId = 1,
            FormId = 1,
            TenantName = "Test Tenant",
            StorageUrl = "test-url",
            Version = 1,
            CreatedBy = "test@example.com",
            Designers = new List<Data.Models.Designer>
            {
                new() { DesignerId = 1, FormDesignId = "test-id" }
            },
            Processors = new List<Data.Models.Processor>
            {
                new() { ProcessorId = 1, FormDesignId = "test-id" }
            },
            FormStates = new List<Data.Models.FormStatesConfig>
            {
                new() { Label = "Test", Value = "Value", FormDesignId = "test-id" }
            }
        };

        _formDesignerHistoryRepositoryMock.Setup(x => x.SaveFormDesignHistoryAsync(It.IsAny<Data.Models.FormDesignHistory>()))
            .ReturnsAsync((Data.Models.FormDesignHistory history) => history);

        // Act
        await _formDesignerHistoryService.SaveFormDesignVersionHistory(formDesign);

        // Assert
        _formDesignerHistoryRepositoryMock.Verify(x => x.SaveFormDesignHistoryAsync(It.Is<Data.Models.FormDesignHistory>(h =>
            h.FormDesignId == formDesign.Id &&
            h.Name == formDesign.Name &&
            h.TenantId == formDesign.TenantId &&
            h.FormId == formDesign.FormId &&
            h.TenantName == formDesign.TenantName &&
            h.StorageUrl == formDesign.StorageUrl &&
            h.Version == formDesign.Version &&
            h.CreatedBy == formDesign.CreatedBy &&
            h.Designers.Count == formDesign.Designers.Count &&
            h.Processors.Count == formDesign.Processors.Count &&
            h.FormStates.Count == formDesign.FormStates.Count
        )), Times.Once);
    }
} 