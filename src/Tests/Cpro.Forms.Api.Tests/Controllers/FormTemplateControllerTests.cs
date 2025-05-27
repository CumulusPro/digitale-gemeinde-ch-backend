using Cpro.Forms.Api.Controllers;
using Cpro.Forms.Service.Models;
using Cpro.Forms.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Peritos.Common.Abstractions.Paging;

namespace Cpro.Forms.Api.Tests.Controllers;

public class FormTemplateControllerTests
{
    private readonly Mock<IFormTemplateService> _formTemplateServiceMock;
    private readonly FormTemplateController _controller;

    public FormTemplateControllerTests()
    {
        _formTemplateServiceMock = new Mock<IFormTemplateService>();
        _controller = new FormTemplateController(_formTemplateServiceMock.Object);
    }

    [Fact]
    public async Task GetFormTemplate_ReturnsOkResult_WhenTemplateExists()
    {
        // Arrange
        var templateId = "test-template-id";
        var tenantId = 1;
        var expectedTemplate = new FormTemplate();
        _formTemplateServiceMock.Setup(x => x.GetFormTemplate(templateId))
            .ReturnsAsync(expectedTemplate);

        // Act
        var result = await _controller.GetFormTemplate(templateId, tenantId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<FormTemplate>(okResult.Value);
        Assert.Equal(expectedTemplate, returnValue);
    }

    [Fact]
    public async Task CreateFormTemplate_ReturnsOkResult_WhenTemplateCreated()
    {
        // Arrange
        var templateRequest = new CreateFormTemplateRequest();
        var tenantId = 1;
        var expectedTemplate = new FormTemplate();
        _formTemplateServiceMock.Setup(x => x.CreateFormTemplate(templateRequest, tenantId))
            .ReturnsAsync(expectedTemplate);

        // Act
        var result = await _controller.CreateFormTemplate(templateRequest, tenantId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<FormTemplate>(okResult.Value);
        Assert.Equal(expectedTemplate, returnValue);
    }

    [Fact]
    public async Task UpdateFormTemplate_ReturnsOkResult_WhenTemplateUpdated()
    {
        // Arrange
        var templateId = "test-template-id";
        var template = new FormTemplate();
        var tenantId = 1;
        var expectedTemplate = new FormTemplate();
        _formTemplateServiceMock.Setup(x => x.UpdateFormTemplate(templateId, template, tenantId))
            .ReturnsAsync(expectedTemplate);

        // Act
        var result = await _controller.UpdateFormTemplate(template, templateId, tenantId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<FormTemplate>(okResult.Value);
        Assert.Equal(expectedTemplate, returnValue);
    }

    [Fact]
    public async Task DeleteFormTemplate_ReturnsNoContent_WhenTemplateDeleted()
    {
        // Arrange
        var templateId = "test-template-id";
        var tenantId = 1;
        var template = new FormTemplate();
        _formTemplateServiceMock.Setup(x => x.GetFormTemplate(templateId))
            .ReturnsAsync(template);

        // Act
        var result = await _controller.DeleteFormTemplate(templateId, tenantId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task SearchFormTemplate_ReturnsOkResult_WhenTemplatesFound()
    {
        // Arrange
        var searchRequest = new SearchRequest();
        var tenantId = 1;
        var expectedResponse = new PagingResponse<FormTemplate>();
        _formTemplateServiceMock.Setup(x => x.SearchFormTemplate(searchRequest))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.SearchFormTemplate(searchRequest, tenantId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<PagingResponse<FormTemplate>>(okResult.Value);
        Assert.Equal(expectedResponse, returnValue);
    }
} 