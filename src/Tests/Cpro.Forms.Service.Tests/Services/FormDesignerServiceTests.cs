using Cpro.Forms.Data.Repositories;
using Cpro.Forms.Integration.Storage.Services;
using Cpro.Forms.Service.Models;
using Cpro.Forms.Service.Services;
using Newtonsoft.Json;
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
            designers = new List<string> { "designer1@test.com" },
            processors = new List<string> { "processor1@test.com" },
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
        var email = "test@example.com";
        var expectedDataModel = new Data.Models.SearchRequest();
        var expectedFormDesigns = new PagingResponse<Data.Models.FormDesign>();
        var expectedResponse = new PagingResponse<FormDesign>();

        _mapperMock.Setup(x => x.Map<Data.Models.SearchRequest>(searchRequest))
            .Returns(expectedDataModel);
        _formDesignRepositoryMock.Setup(x => x.SearchFormDesignsAsync(expectedDataModel, tenantId, email))
            .ReturnsAsync(expectedFormDesigns);
        _mapperMock.Setup(x => x.Map<PagingResponse<FormDesign>>(expectedFormDesigns))
            .Returns(expectedResponse);

        // Act
        var result = await _formDesignerService.SearchFormDesignsAsync(searchRequest, tenantId, email);

        // Assert
        Assert.Equal(expectedResponse, result);
        _mapperMock.Verify(x => x.Map<Data.Models.SearchRequest>(searchRequest), Times.Once);
        _formDesignRepositoryMock.Verify(x => x.SearchFormDesignsAsync(expectedDataModel, tenantId, email), Times.Once);
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

    [Fact]
    public async Task CreateFormDefinitionAsync_UpdatesExistingForm_WhenFormExists()
    {
        // Arrange
        var existingForm = new Data.Models.FormDesign
        {
            Id = "existing-id",
            FormId = 2,
            StorageUrl = "existing-id/v1.json"
        };

        var fieldRequest = new FieldRequest
        {
            Name = "Updated Form",
            Fields = new List<Fields>
        {
            new Fields { Id = null, Name = "Field1", DisplayName = "Field 1", IsRequired = true, Datatype = "string" }
        },
            designers = new List<string>(),
            processors = new List<string>(),
            formStatesConfig = new List<FormStatesConfig>()
        };

        _formDesignRepositoryMock.Setup(x => x.GetFormDesign(existingForm.Id, It.IsAny<int>()))
            .ReturnsAsync(existingForm);

        _formDesignRepositoryMock.Setup(x => x.UpdateFormDesignAsync(existingForm.Id, It.IsAny<Data.Models.FormDesign>()))
            .ReturnsAsync(existingForm);

        _azureBlobServiceMock.Setup(x => x.UploadFile(It.IsAny<string>(), It.IsAny<byte[]>()))
            .Returns(Task.CompletedTask);

        _formDesignHistoryServiceMock.Setup(x => x.SaveFormDesignVersionHistory(existingForm))
            .Returns(Task.CompletedTask);

        _azureBlobServiceMock.Setup(x => x.GetSignedUrl(existingForm.StorageUrl))
            .Returns("signed-url");

        _mapperMock.Setup(x => x.Map<FormDesign>(existingForm))
            .Returns(new FormDesign());

        // Act
        var result = await _formDesignerService.CreateFormDefinitionAsync(fieldRequest, existingForm.Id, 1, "email@test.com");

        // Assert
        _formDesignRepositoryMock.Verify(x => x.UpdateFormDesignAsync(existingForm.Id, It.IsAny<Data.Models.FormDesign>()), Times.Once);
        _formDesignHistoryServiceMock.Verify(x => x.SaveFormDesignVersionHistory(existingForm), Times.Once);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetFormDefinitionResponseAsync_ReturnsDocumentResponseWithTenantDesignApplied()
    {
        // Arrange
        var formId = "form1";
        int tenantId = 123;

        var formDesign = new Data.Models.FormDesign
        {
            Id = formId,
            StorageUrl = $"{formId}/v1.json",
            IsActive = true
        };

        var documentResponse = new DocumentResponse
        {
            useTenantDesign = true,
            header = new Header { content = "header content" },
            footer = new Footer { content = "footer content" }
        };

        var tenantDesign = new TenantDesign
        {
            header = new Header { content = "tenant header" },
            footer = new Footer { content = "tenant footer" },
            designConfig = new DesignConfig()
        };

        _formDesignRepositoryMock.Setup(x => x.GetFormDesign(formId, tenantId))
            .ReturnsAsync(formDesign);

        _azureBlobServiceMock.SetupSequence(x => x.GetFile(It.IsAny<string>()))
            .ReturnsAsync(JsonConvert.SerializeObject(documentResponse))
            .ReturnsAsync(JsonConvert.SerializeObject(tenantDesign));

        // Act
        var result = await _formDesignerService.GetFormDefinitionResponseAsync(formId, tenantId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(tenantDesign.header.content, result.header.content);
        Assert.Equal(tenantDesign.footer.content, result.footer.content);
        Assert.Equal(formDesign.IsActive, result.IsActive);
    }

    [Fact]
    public async Task GetFormDesignsByTenantIdAsync_ReturnsFormDesignsWithSignedUrls()
    {
        // Arrange
        int tenantId = 1;

        var formDesigns = new List<Data.Models.FormDesign>
        {
            new Data.Models.FormDesign { StorageUrl = "url1" },
            new Data.Models.FormDesign { StorageUrl = "url2" }
        };

        _formDesignRepositoryMock.Setup(x => x.GetFormDesignsByTenantId(tenantId))
            .ReturnsAsync(formDesigns);

        _azureBlobServiceMock.Setup(x => x.GetSignedUrl(It.IsAny<string>()))
            .Returns<string>(url => "signed-" + url);

        _mapperMock.Setup(x => x.Map<List<FormDesign>>(formDesigns))
            .Returns(new List<FormDesign>());

        // Act
        var result = await _formDesignerService.GetFormDesignsByTenantIdAsync(tenantId);

        // Assert
        _azureBlobServiceMock.Verify(x => x.GetSignedUrl("url1"), Times.Once);
        _azureBlobServiceMock.Verify(x => x.GetSignedUrl("url2"), Times.Once);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task DuplicateFormDefinitionAsync_DuplicatesFormSuccessfully()
    {
        // Arrange
        var formId = "original-form-id";
        var email = "test@example.com";

        var originalForm = new Data.Models.FormDesign
        {
            Id = "original-id",
            FormId = 1,
            StorageUrl = "original-id/v1.json",
            Name = "Original Form",
            TenantId = 1,
            Version = 1,
            Designers = new List<Data.Models.Designer>
            {
                new Data.Models.Designer { Email = "designer1@test.com", FormDesignId = formId }
            },
            Processors = new List<Data.Models.Processor>
            {
                new Data.Models.Processor { Email = "processor1@test.com", FormDesignId = formId }
            },
            FormStates = new List<Data.Models.FormStatesConfig>
            {
                new Data.Models.FormStatesConfig { Label = "Approved", Value = "approved", FormDesignId = formId }
            },
            Tags = new List<Data.Models.FormDesignTag>
            {
                new Data.Models.FormDesignTag { TagId = 1, FormDesignId = formId }
            }
        };

        var duplicatedForm = new Data.Models.FormDesign
        {
            Id = "new-id",
            FormId = 2,
            StorageUrl = "new-id/v1.json",
            Name = "Original Form_20250101120000",
            TenantId = 1,
            Version = 1,
            Designers = new List<Data.Models.Designer>
            {
                new Data.Models.Designer { Email = "designer1@test.com", FormDesignId = formId }
            },
            Processors = new List<Data.Models.Processor>
            {
                new Data.Models.Processor { Email = "processor1@test.com", FormDesignId = formId }
            },
            FormStates = new List<Data.Models.FormStatesConfig>
            {
                new Data.Models.FormStatesConfig { Label = "Approved", Value = "approved", FormDesignId = formId }
            },
            Tags = new List<Data.Models.FormDesignTag>
            {
                new Data.Models.FormDesignTag { TagId = 1, FormDesignId = formId }
            }
        };

        var fieldRequest = new FieldRequest
        {
            Id = originalForm.FormId,
            Name = originalForm.Name,
            Fields = new List<Fields>()
        };

        _formDesignRepositoryMock.Setup(x => x.GetFormDesignByFormId(formId))
            .ReturnsAsync(originalForm);

        _formDesignRepositoryMock.Setup(x => x.GetFormDesignCountAsync())
            .ReturnsAsync(1);

        _formDesignRepositoryMock.Setup(x => x.CreateFormDesignAsync(It.IsAny<Data.Models.FormDesign>()))
            .ReturnsAsync(duplicatedForm);

        _azureBlobServiceMock.Setup(x => x.GetFile(originalForm.StorageUrl))
            .ReturnsAsync(JsonConvert.SerializeObject(fieldRequest));

        _azureBlobServiceMock.Setup(x => x.UploadFile(duplicatedForm.StorageUrl, It.IsAny<byte[]>()))
            .Returns(Task.CompletedTask);

        _formDesignHistoryServiceMock.Setup(x => x.SaveFormDesignVersionHistory(duplicatedForm))
            .Returns(Task.CompletedTask);

        _azureBlobServiceMock.Setup(x => x.GetSignedUrl(duplicatedForm.StorageUrl))
            .Returns("signed-url");

        _mapperMock.Setup(x => x.Map<FormDesign>(duplicatedForm))
            .Returns(new FormDesign());

        // Act
        var result = await _formDesignerService.DuplicateFormDefinitionAsync(formId, email);

        // Assert
        Assert.NotNull(result);
        _formDesignRepositoryMock.Verify(x => x.CreateFormDesignAsync(It.IsAny<Data.Models.FormDesign>()), Times.Once);
        _formDesignHistoryServiceMock.Verify(x => x.SaveFormDesignVersionHistory(duplicatedForm), Times.Once);
    }

    [Fact]
    public async Task GetAllDistinctTagNamesAsync_ReturnsListOfTags()
    {
        // Arrange
        var expectedTags = new List<string> { "tag1", "tag2" };

        _formDesignRepositoryMock.Setup(x => x.GetAllDistinctTagNamesAsync())
            .ReturnsAsync(expectedTags);

        // Act
        var result = await _formDesignerService.GetAllDistinctTagNamesAsync();

        // Assert
        Assert.Equal(expectedTags, result);
    }

}