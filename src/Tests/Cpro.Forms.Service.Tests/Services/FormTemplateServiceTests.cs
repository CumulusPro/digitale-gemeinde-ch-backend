using Cpro.Forms.Data.Repositories;
using Cpro.Forms.Integration.Storage.Services;
using Cpro.Forms.Service.Models;
using Cpro.Forms.Service.Services;
using Newtonsoft.Json;
using Peritos.Common.Abstractions.Paging;

namespace Cpro.Forms.Service.Tests.Services;

public class FormTemplateServiceTests
{
    private readonly Mock<IFormTemplateRepository> _formTemplateRepositoryMock;
    private readonly Mock<IAzureBlobService> _azureBlobServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly FormTemplateService _formTemplateService;

    public FormTemplateServiceTests()
    {
        _formTemplateRepositoryMock = new Mock<IFormTemplateRepository>();
        _azureBlobServiceMock = new Mock<IAzureBlobService>();
        _mapperMock = new Mock<IMapper>();
        _formTemplateService = new FormTemplateService(_formTemplateRepositoryMock.Object, _mapperMock.Object, _azureBlobServiceMock.Object);
    }

    [Fact]
    public async Task GetFormTemplate_ReturnsTemplate_WhenTemplateExists()
    {
        // Arrange
        var templateId = "test-template-id";
        var template = new Data.Models.FormTemplate
        {
            Id = templateId,
            Name = "Test",
            TenantId = 1,
            Version = 1,
            StorageUrl = $"Templates/{templateId}/v1.json"
        };
        var templateJson = JsonConvert.SerializeObject(template);

        _formTemplateRepositoryMock.Setup(x => x.GetFormTemplate(templateId))
            .ReturnsAsync(template);
        _azureBlobServiceMock.Setup(x => x.GetFile(template.StorageUrl))
            .ReturnsAsync(templateJson);

        // Act
        var result = await _formTemplateService.GetFormTemplate(templateId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(template.Id, result.id);
        Assert.Equal(template.Name, result.Name);
        _formTemplateRepositoryMock.Verify(x => x.GetFormTemplate(templateId), Times.Once);
        _azureBlobServiceMock.Verify(x => x.GetFile(template.StorageUrl), Times.Once);
    }

    [Fact]
    public async Task CreateFormTemplate_CreatesTemplate_WhenValidInput()
    {
        // Arrange
        var request = new CreateFormTemplateRequest { Name = "Test", TenantId = 1 };
        var datamodel = new Data.Models.FormTemplate
        {
            Id = "new-id",
            Name = "Test",
            TenantId = 1
        };
        var mapped = datamodel;
        var created = datamodel;

        var serviceModel = new FormTemplate
        {
            id = "new-id",
            Name = "Test",
            TenantId = 1,
        };

        _mapperMock.Setup(x => x.Map<Data.Models.FormTemplate>(request)).Returns(mapped);
        _formTemplateRepositoryMock.Setup(x => x.CreateFormTemplateAsync(mapped)).ReturnsAsync(created);
        _mapperMock.Setup(x => x.Map<FormTemplate>(created)).Returns(serviceModel);

        // Act
        var result = await _formTemplateService.CreateFormTemplate(request, 1);

        // Assert
        Assert.NotNull(result);        
    }

    [Fact]
    public async Task UpdateFormTemplate_UpdatesTemplate_WhenTemplateExists()
    {
        // Arrange
        var templateId = "test-template-id";
        var tenantId = 1;
        var request = new FormTemplate
        {
            id = templateId,
            Name = "Updated",
            TenantId = tenantId
        };
        var existing = new Data.Models.FormTemplate
        {
            Id = templateId,
            Name = "Old",
            TenantId = tenantId
        };
        var datamodel = new Data.Models.FormTemplate
        {
            Id = templateId,
            Name = "Updated",
            TenantId = tenantId
        };

        _formTemplateRepositoryMock.Setup(x => x.GetFormTemplate(templateId)).ReturnsAsync(existing);
        _mapperMock.Setup(x => x.Map<Data.Models.FormTemplate>(request)).Returns(datamodel);
        _formTemplateRepositoryMock.Setup(x => x.UpdateFormTemplateAsync(templateId, datamodel)).ReturnsAsync(datamodel);
        _mapperMock.Setup(x => x.Map<Data.Models.FormTemplate>(datamodel)).Returns(datamodel);

        // Act
        var result = await _formTemplateService.UpdateFormTemplate(templateId, request, tenantId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteFormTemplate_DeletesTemplate_WhenTemplateExists()
    {
        // Arrange
        var templateId = "test-template-id";

        _azureBlobServiceMock.Setup(x => x.DeleteFolder($"Templates/{templateId}")).Returns(Task.CompletedTask);
        _formTemplateRepositoryMock.Setup(x => x.DeleteFormTemplateAsync(templateId)).ReturnsAsync((Data.Models.FormTemplate)null);

        // Act
        await _formTemplateService.DeleteFormTemplate(templateId);

        // Assert
        _azureBlobServiceMock.Verify(x => x.DeleteFolder($"Templates/{templateId}"), Times.Once);
        _formTemplateRepositoryMock.Verify(x => x.DeleteFormTemplateAsync(templateId), Times.Once);
    }

    [Fact]
    public async Task SearchFormTemplate_ReturnsPagingResponse_WhenTemplatesFound()
    {
        // Arrange
        var searchRequest = new SearchRequest { Keyword = "test" };
        var datamodel = new Data.Models.SearchRequest { Keyword = "test" };
        var repoResponse = new PagingResponse<Data.Models.FormTemplate> { Data = new List<Data.Models.FormTemplate> { new Data.Models.FormTemplate { Id = "1" } }, TotalCount = 1 };
        var mappedResponse = new PagingResponse<FormTemplate> { Data = new List<FormTemplate> { new FormTemplate { id = "1" } }, TotalCount = 1 };

        _mapperMock.Setup(x => x.Map<Data.Models.SearchRequest>(searchRequest)).Returns(datamodel);
        _formTemplateRepositoryMock.Setup(x => x.SearchFormTemplatesAsync(datamodel)).ReturnsAsync(repoResponse);
        _mapperMock.Setup(x => x.Map<PagingResponse<FormTemplate>>(repoResponse)).Returns(mappedResponse);

        // Act
        var result = await _formTemplateService.SearchFormTemplate(searchRequest);

        // Assert
        Assert.NotNull(result);
    }
} 