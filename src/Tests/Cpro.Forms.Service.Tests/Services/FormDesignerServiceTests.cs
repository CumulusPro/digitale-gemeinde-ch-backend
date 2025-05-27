using Cpro.Forms.Data.Repositories;
using Cpro.Forms.Integration.Storage.Services;
using Cpro.Forms.Service.Models;
using Cpro.Forms.Service.Services;
using Peritos.Common.Abstractions.Paging;

namespace Cpro.Forms.Service.Tests.Services;

public class FormDesignerServiceTests
{
    private readonly Mock<IFormDesignRepository> _formDesignRepositoryMock;
    private readonly Mock<IFormDesignerHistoryService> _formDesignHistoryServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IAzureBlobService> _azureBlobServiceMock;
    private readonly FormDesignerService _formDesignerService;

    public FormDesignerServiceTests()
    {
        _formDesignRepositoryMock = new Mock<IFormDesignRepository>();
        _formDesignHistoryServiceMock = new Mock<IFormDesignerHistoryService>();
        _mapperMock = new Mock<IMapper>();
        _azureBlobServiceMock = new Mock<IAzureBlobService>();
        
        _formDesignerService = new FormDesignerService(
            _formDesignRepositoryMock.Object,
            _formDesignHistoryServiceMock.Object,
            _mapperMock.Object,
            _azureBlobServiceMock.Object);
    }

    [Fact]
    public async Task CreateFormDefinitionAsync_CreatesNewForm_WhenFormDoesNotExist()
    {
        // Arrange
        var fieldRequest = new FieldRequest
        {
            Name = "Test Form",
            Fields = new List<Fields>(),
            designers = new List<int> { 1 },
            processors = new List<int> { 1 },
            formStatesConfig = new List<FormStatesConfig>()
        };
        var formId = string.Empty;
        var tenantId = 1;
        var email = "test@example.com";
        var expectedFormDesign = new Data.Models.FormDesign
        {
            Id = "test-id",
            Name = fieldRequest.Name,
            TenantId = tenantId,
            FormId = 1,
            Version = 1,
            StorageUrl = "test-id/v1.json",
            CreatedBy = email
        };
        var expectedResponse = new FormDesign();

        _formDesignRepositoryMock.Setup(x => x.GetFormDesign(formId, tenantId))
            .ReturnsAsync((Data.Models.FormDesign)null);
        _formDesignRepositoryMock.Setup(x => x.GetFormDesignCountAsync())
            .ReturnsAsync(0);
        _formDesignRepositoryMock.Setup(x => x.CreateFormDesignAsync(It.IsAny<Data.Models.FormDesign>()))
            .ReturnsAsync(expectedFormDesign);
        _mapperMock.Setup(x => x.Map<FormDesign>(expectedFormDesign))
            .Returns(expectedResponse);

        // Act
        var result = await _formDesignerService.CreateFormDefinitionAsync(fieldRequest, formId, tenantId, email);

        // Assert
        Assert.Equal(expectedResponse, result);
        _formDesignRepositoryMock.Verify(x => x.CreateFormDesignAsync(It.IsAny<Data.Models.FormDesign>()), Times.Once);
        _formDesignHistoryServiceMock.Verify(x => x.SaveFormDesignVersionHistory(It.IsAny<Data.Models.FormDesign>()), Times.Once);
    }

    [Fact]
    public async Task GetFormDataJsonAsync_ReturnsSignedUrl_WhenFormExists()
    {
        // Arrange
        var formId = "test-form-id";
        var expectedFormDesign = new Data.Models.FormDesign
        {
            StorageUrl = "test-url"
        };
        var expectedSignedUrl = "signed-url";

        _formDesignRepositoryMock.Setup(x => x.GetFormDesignByFormId(formId))
            .ReturnsAsync(expectedFormDesign);
        _azureBlobServiceMock.Setup(x => x.GetSignedUrl(expectedFormDesign.StorageUrl))
            .Returns(expectedSignedUrl);

        // Act
        var result = await _formDesignerService.GetFormDataJsonAsync(formId);

        // Assert
        Assert.Equal(expectedSignedUrl, result);
        _formDesignRepositoryMock.Verify(x => x.GetFormDesignByFormId(formId), Times.Once);
        _azureBlobServiceMock.Verify(x => x.GetSignedUrl(expectedFormDesign.StorageUrl), Times.Once);
    }

    [Fact]
    public async Task GetFormDataJsonAsync_ThrowsFileNotFoundException_WhenFormDoesNotExist()
    {
        // Arrange
        var formId = "test-form-id";

        _formDesignRepositoryMock.Setup(x => x.GetFormDesignByFormId(formId))
            .ReturnsAsync((Data.Models.FormDesign)null);

        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(() => 
            _formDesignerService.GetFormDataJsonAsync(formId));
    }

    [Fact]
    public async Task DeleteFormDesignAsync_DeletesFormAndFolder_WhenFormExists()
    {
        // Arrange
        var formId = "test-form-id";
        var tenantId = 1;

        // Act
        await _formDesignerService.DeleteFormDesignAsync(formId, tenantId);

        // Assert
        _azureBlobServiceMock.Verify(x => x.DeleteFolder(formId), Times.Once);
        _formDesignRepositoryMock.Verify(x => x.DeleteFormDesignAsync(formId, tenantId), Times.Once);
    }

    [Fact]
    public async Task SearchFormDesignsAsync_ReturnsPagingResponse_WhenFormsFound()
    {
        // Arrange
        var searchRequest = new SearchRequest();
        var tenantId = 1;
        var expectedDataModel = new Data.Models.SearchRequest();
        var expectedFormDesigns = new PagingResponse<Data.Models.FormDesign>();
        var expectedResponse = new PagingResponse<FormDesign>();

        _mapperMock.Setup(x => x.Map<Data.Models.SearchRequest>(searchRequest))
            .Returns(expectedDataModel);
        _formDesignRepositoryMock.Setup(x => x.SearchFormDesignsAsync(expectedDataModel, tenantId))
            .ReturnsAsync(expectedFormDesigns);
        _mapperMock.Setup(x => x.Map<PagingResponse<FormDesign>>(expectedFormDesigns))
            .Returns(expectedResponse);

        // Act
        var result = await _formDesignerService.SearchFormDesignsAsync(searchRequest, tenantId);

        // Assert
        Assert.Equal(expectedResponse, result);
        _mapperMock.Verify(x => x.Map<Data.Models.SearchRequest>(searchRequest), Times.Once);
        _formDesignRepositoryMock.Verify(x => x.SearchFormDesignsAsync(expectedDataModel, tenantId), Times.Once);
        _mapperMock.Verify(x => x.Map<PagingResponse<FormDesign>>(expectedFormDesigns), Times.Once);
    }

    [Fact]
    public async Task ActivateFormDefinitionAsync_UpdatesForm_WhenFormExists()
    {
        // Arrange
        var formId = "test-form-id";
        var isActive = true;
        var tenantId = 1;
        var formDesign = new Data.Models.FormDesign();

        _formDesignRepositoryMock.Setup(x => x.GetFormDesign(formId, tenantId))
            .ReturnsAsync(formDesign);

        // Act
        await _formDesignerService.ActivateFormDefinitionAsync(formId, isActive, tenantId);

        // Assert
        Assert.Equal(isActive, formDesign.IsActive);
        _formDesignRepositoryMock.Verify(x => x.UpdateFormDesignAsync(formId, formDesign), Times.Once);
    }

    [Fact]
    public async Task ActivateFormDefinitionAsync_ThrowsFileNotFoundException_WhenFormDoesNotExist()
    {
        // Arrange
        var formId = "test-form-id";
        var isActive = true;
        var tenantId = 1;

        _formDesignRepositoryMock.Setup(x => x.GetFormDesign(formId, tenantId))
            .ReturnsAsync((Data.Models.FormDesign)null);

        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(() => 
            _formDesignerService.ActivateFormDefinitionAsync(formId, isActive, tenantId));
    }
} 