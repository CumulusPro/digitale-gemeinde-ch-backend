using Cpro.Forms.Api.Controllers;
using Cpro.Forms.Service.Models;
using Cpro.Forms.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Peritos.Common.Abstractions;
using Peritos.Common.Abstractions.Paging;

namespace Cpro.Forms.Api.Tests.Controllers;

public class FormControllerTests
{
    private readonly Mock<IFormService> _formServiceMock;
    private readonly Mock<IRequestContext> _requestContextMock;
    private readonly FormController _controller;

    public FormControllerTests()
    {
        _formServiceMock = new Mock<IFormService>();
        _requestContextMock = new Mock<IRequestContext>();
        _controller = new FormController(_formServiceMock.Object, _requestContextMock.Object);
    }

    [Fact]
    public async Task GetFormData_ReturnsOkResult_WhenFormExists()
    {
        // Arrange
        var formId = "test-form-id";
        var tenantId = 1;
        var documentId = "test-document-id";
        var expectedResponse = new DocumentResponse();
        _formServiceMock.Setup(x => x.GetFormDataAsync(formId, tenantId, documentId))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetFormData(formId, tenantId, documentId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<DocumentResponse>(okResult.Value);
        Assert.Equal(expectedResponse, returnValue);
    }

    [Fact]
    public async Task SubmitForm_ReturnsOkResult_WhenFormSubmitted()
    {
        // Arrange
        var jsonData = new { };
        var origin = "test-origin";
        var expectedResponse = new FormResponse();
        _formServiceMock.Setup(x => x.SubmitTaskAsync(jsonData, origin))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.SubmitForm(jsonData, origin);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<FormResponse>(okResult.Value);
        Assert.Equal(expectedResponse, returnValue);
    }

    [Fact]
    public async Task UpdateFormData_ReturnsOkResult_WhenFormUpdated()
    {
        // Arrange
        var request = new StatusRequest();
        var formId = "test-form-id";
        var documentId = "test-document-id";
        var tenantId = 1;
        var expectedResponse = new DocumentResponse();
        _formServiceMock.Setup(x => x.UpdateFormStatus(tenantId, formId, documentId, request.status))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.UpdateFormData(request, formId, documentId, tenantId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<DocumentResponse>(okResult.Value);
        Assert.Equal(expectedResponse, returnValue);
    }

    [Fact]
    public async Task GetFormNavigation_ReturnsOkResult_WhenNavigationFound()
    {
        // Arrange
        var tenantId = 1;
        var userEmail = "test@example.com";
        var expectedResponse = new List<FormNavigation>();
        _requestContextMock.Setup(x => x.UserEmail).Returns(userEmail);
        _formServiceMock.Setup(x => x.GetFormNavigationAsync(tenantId, userEmail))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetFormNavigation(tenantId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<FormNavigation>>(okResult.Value);
        Assert.Equal(expectedResponse, returnValue);
    }

    [Fact]
    public async Task SearchFormData_ReturnsOkResult_WhenFormsFound()
    {
        // Arrange
        var searchRequest = new FormSearchRequest();
        var tenantId = 1;
        var expectedResponse = new PagingResponse<FormData>();
        _formServiceMock.Setup(x => x.SearchFormData(searchRequest, tenantId))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.SearchFormData(searchRequest, tenantId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<PagingResponse<FormData>>(okResult.Value);
        Assert.Equal(expectedResponse, returnValue);
    }
} 