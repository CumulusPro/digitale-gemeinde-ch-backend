using Cpro.Forms.Api.Controllers;
using Cpro.Forms.Service.Models;
using Cpro.Forms.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Peritos.Common.Abstractions;
using Peritos.Common.Abstractions.Paging;
using System.Text;

namespace Cpro.Forms.Api.Tests.Controllers;

public class FormDefinitionControllerTests
{
    private readonly Mock<IFormDesignerService> _formServiceMock;
    private readonly Mock<IFormDesignerHistoryService> _formHistoryServiceMock;
    private readonly Mock<IRequestContext> _requestContextMock;
    private readonly FormDefinitionController _controller;

    public FormDefinitionControllerTests()
    {
        _formServiceMock = new Mock<IFormDesignerService>();
        _formHistoryServiceMock = new Mock<IFormDesignerHistoryService>();
        _requestContextMock = new Mock<IRequestContext>();
        _controller = new FormDefinitionController(_formServiceMock.Object, _formHistoryServiceMock.Object, _requestContextMock.Object);
    }

    [Fact]
    public async Task GetFormDefinition_ReturnsOkResult_WhenFormExists()
    {
        // Arrange
        var formId = "test-form-id";
        var tenantId = 1;
        var expectedResponse = new DocumentResponse();
        _formServiceMock.Setup(x => x.GetFormDefinitionResponseAsync(formId, tenantId))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetFormDefinition(formId, tenantId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<DocumentResponse>(okResult.Value);
        Assert.Equal(expectedResponse, returnValue);
    }

    [Fact]
    public async Task CreateFormDefinition_ReturnsOkResult_WhenFormCreated()
    {
        // Arrange
        var fieldRequest = new FieldRequest();
        var tenantId = 1;
        var userEmail = "test@example.com";
        var expectedResponse = new FormDesign();
        _requestContextMock.Setup(x => x.UserEmail).Returns(userEmail);
        _formServiceMock.Setup(x => x.CreateFormDefinitionAsync(fieldRequest, string.Empty, tenantId, userEmail))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.CreateFormDefinition(fieldRequest, tenantId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<FormDesign>(okResult.Value);
        Assert.Equal(expectedResponse, returnValue);
    }

    [Fact]
    public async Task UpdateFormDefinition_ReturnsOkResult_WhenFormUpdated()
    {
        // Arrange
        var fieldRequest = new FieldRequest();
        var formId = "test-form-id";
        var tenantId = 1;
        var userEmail = "test@example.com";
        var expectedResponse = new FormDesign();
        _requestContextMock.Setup(x => x.UserEmail).Returns(userEmail);
        _formServiceMock.Setup(x => x.CreateFormDefinitionAsync(fieldRequest, formId, tenantId, userEmail))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.UpdateFormDefinition(fieldRequest, tenantId, formId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<FormDesign>(okResult.Value);
        Assert.Equal(expectedResponse, returnValue);
    }

    [Fact]
    public async Task ActivateFormDefinition_ReturnsNoContent_WhenFormActivated()
    {
        // Arrange
        var formId = "test-form-id";
        var isActive = true;
        var tenantId = 1;

        // Act
        var result = await _controller.ActivateFormDefinition(formId, isActive, tenantId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _formServiceMock.Verify(x => x.ActivateFormDefinitionAsync(formId, isActive, tenantId), Times.Once);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenFormDeleted()
    {
        // Arrange
        var formId = "test-form-id";
        var tenantId = 1;

        // Act
        var result = await _controller.Delete(formId, tenantId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _formServiceMock.Verify(x => x.DeleteFormDesignAsync(formId, tenantId), Times.Once);
    }

    [Fact]
    public async Task SearchFormDesigns_ReturnsOkResult_WhenFormsFound()
    {
        // Arrange
        var searchRequest = new SearchRequest();
        var tenantId = 1;
        var expectedResponse = new PagingResponse<FormDesign>();
        _formServiceMock.Setup(x => x.SearchFormDesignsAsync(searchRequest, tenantId))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.SearchFormDesigns(searchRequest, tenantId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<PagingResponse<FormDesign>>(okResult.Value);
        Assert.Equal(expectedResponse, returnValue);
    }

    [Fact]
    public async Task DuplicateFormDefinition_ReturnsOkResult_WhenFormDuplicated()
    {
        // Arrange
        var formId = "test-form-id";
        var userEmail = "test@example.com";
        var expectedResponse = new FormDesign();
        _requestContextMock.Setup(x => x.UserEmail).Returns(userEmail);
        _formServiceMock.Setup(x => x.DuplicateFormDefinitionAsync(formId, userEmail))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.DuplicateFormDefinition(formId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<FormDesign>(okResult.Value);
        Assert.Equal(expectedResponse, returnValue);
    }

    [Fact]
    public async Task DuplicateFormDefinition_ReturnsBadRequest_WhenFormIdIsEmpty()
    {
        // Arrange
        var formId = "";

        // Act
        var result = await _controller.DuplicateFormDefinition(formId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("FormId and TenantId are required.", badRequestResult.Value);
    }

    [Fact]
    public async Task ExportForm_ReturnsOkResult_WhenFormExported()
    {
        // Arrange
        var formId = "test-form-id";
        var expectedResponse = "exported-form-data";
        _formServiceMock.Setup(x => x.GetFormDataJsonAsync(formId))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.ExportForm(formId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedResponse, okResult.Value);
    }

    [Fact]
    public async Task ExportForm_ReturnsBadRequest_WhenFormIdIsEmpty()
    {
        // Arrange
        var formId = "";

        // Act
        var result = await _controller.ExportForm(formId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("FormId is required.", badRequestResult.Value);
    }

    [Fact]
    public async Task ImportForm_ReturnsOkResult_WhenFormImported()
    {
        // Arrange
        var fileContent = "{\"Field1\":\"value\"}";
        var fileName = "form.json";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
        var formFileMock = new Mock<IFormFile>();
        formFileMock.Setup(f => f.OpenReadStream()).Returns(stream);
        formFileMock.Setup(f => f.FileName).Returns(fileName);
        formFileMock.Setup(f => f.Length).Returns(stream.Length);
        formFileMock.Setup(f => f.ContentType).Returns("application/json");

        var request = new ImportFormRequest
        {
            File = formFileMock.Object,
            ExistingFormId = "test-form-id",
            tenantId = 1
        };
        var userEmail = "test@example.com";
        var expectedResponse = new FormDesign();
        _requestContextMock.Setup(x => x.UserEmail).Returns(userEmail);
        _formServiceMock.Setup(x => x.CreateFormDefinitionAsync(It.IsAny<FieldRequest>(), request.ExistingFormId, request.tenantId, userEmail))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.ImportForm(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<FormDesign>(okResult.Value);
        Assert.Equal(expectedResponse, returnValue);
    }

    [Fact]
    public async Task GetAllVersions_ReturnsOkResult_WhenVersionsFound()
    {
        // Arrange
        var formId = "test-form-id";
        var expectedResponse = new List<FormDesign>();
        _formHistoryServiceMock.Setup(x => x.GetAllVersions(formId))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetAllVersions(formId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<FormDesign>>(okResult.Value);
        Assert.Equal(expectedResponse, returnValue);
    }

    [Fact]
    public async Task RestoreVersion_ReturnsOkResult_WhenVersionRestored()
    {
        // Arrange
        var formId = "test-form-id";
        var version = 1;
        var tenantId = 1;
        var userEmail = "test@example.com";
        var fieldRequest = new FieldRequest();
        var expectedResponse = new FormDesign();
        _requestContextMock.Setup(x => x.UserEmail).Returns(userEmail);
        _formHistoryServiceMock.Setup(x => x.GetVersion(formId, version))
            .ReturnsAsync(fieldRequest);
        _formServiceMock.Setup(x => x.CreateFormDefinitionAsync(fieldRequest, formId, tenantId, userEmail))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.RestoreVersion(formId, version, tenantId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<FormDesign>(okResult.Value);
        Assert.Equal(expectedResponse, returnValue);
    }

    [Fact]
    public async Task GetAllDistinctTags_ReturnsOkResult_WithTagList()
    {
        // Arrange
        var expectedTags = new List<string> { "HR", "Finance", "IT" };
        _formServiceMock.Setup(x => x.GetAllDistinctTagNamesAsync())
            .ReturnsAsync(expectedTags);

        // Act
        var result = await _controller.GetAllDistinctTags();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<string>>(okResult.Value);
        Assert.Equal(expectedTags.Count, returnValue.Count);
        Assert.Equal(expectedTags, returnValue);
    }
}