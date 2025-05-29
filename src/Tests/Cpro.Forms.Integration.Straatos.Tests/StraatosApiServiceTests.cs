using Cpro.Forms.Integration.Storage.Services;
using Cpro.Forms.Integration.Straatos.Configuration;
using Cpro.Forms.Integration.Straatos.Services;
using Cpro.Forms.Service.Models;
using Moq;
using Newtonsoft.Json;
using System.Dynamic;
using Xunit;

namespace Cpro.Forms.Integration.Straatos.Tests;

public class StraatosApiServiceTests
{
    private readonly Mock<IStraatosConfiguration> _configMock;
    private readonly Mock<IHttpService> _httpServiceMock;
    private readonly Mock<IAzureBlobService> _azureBlobServiceMock;
    private readonly StraatosApiService _service;

    public StraatosApiServiceTests()
    {
        _configMock = new Mock<IStraatosConfiguration>();
        _httpServiceMock = new Mock<IHttpService>();
        _azureBlobServiceMock = new Mock<IAzureBlobService>();

        // Setup default configuration
        _configMock.Setup(x => x.BaseUrl).Returns("https://test-api.example.com");
        _configMock.Setup(x => x.WorkflowStepId).Returns("123");
        _configMock.Setup(x => x.AuthToken).Returns("test-token");

        _service = new StraatosApiService(_configMock.Object, _httpServiceMock.Object, _azureBlobServiceMock.Object);
    }

    [Fact]
    public async Task GetCurrentUser_ReturnsUserData()
    {
        // Arrange
        var token = "test-token";
        var expectedResponse = "{\"id\":\"123\",\"name\":\"Test User\"}";
        _httpServiceMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>(), token))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetCurrentUser(token);

        // Assert
        Assert.Equal(expectedResponse, result);
        _httpServiceMock.Verify(x => x.GetAsync(
            It.Is<string>(url => url.Contains("/IAM/Users/CurrentUser")),
            It.IsAny<string>(),
            token), Times.Once);
    }

    [Fact]
    public async Task GetTenantDetails_ReturnsTenantData()
    {
        // Arrange
        var tenantId = 1;
        var expectedResponse = "{\"id\":1,\"name\":\"Test Tenant\"}";
        _httpServiceMock.Setup(x => x.GetAsync(It.IsAny<string>(), null, null))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetTenantDetails(tenantId);

        // Assert
        Assert.Equal(expectedResponse, result);
        _httpServiceMock.Verify(x => x.GetAsync(
            It.Is<string>(url => url.Contains($"/IAM/Tenants/{tenantId}")), null, null), Times.Once);
    }

    [Fact]
    public async Task UploadSimple_WithValidData_ReturnsDocumentId()
    {
        // Arrange
        dynamic jsonData = new ExpandoObject();
        jsonData.tenantId = "1";
        jsonData.data = new ExpandoObject();
        jsonData.data.FormId = "form-123";
        jsonData.data.documentId = "doc-123";

        var documentResponse = new DocumentResponse
        {
            Id = 1,
            DocumentTypeName = "Test Document",
            Name = "Test Name",
            Category = "Test Category",
            Fields = new List<IndexField>()
        };

        var uploadId = "upload-123";
        var documentId = "doc-456";
        var uploadUrl = "https://upload-url.com";
        var completeUrl = "https://complete-url.com";

        _httpServiceMock.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>(), "Initiate"))
            .ReturnsAsync(JsonConvert.SerializeObject(uploadId));

        _httpServiceMock.Setup(x => x.GetAsync(It.Is<string>(url => url.Contains("file/url")), "Upload", null))
            .ReturnsAsync(JsonConvert.SerializeObject(uploadUrl));

        _httpServiceMock.Setup(x => x.GetAsync(It.Is<string>(url => url.Contains("complete")), "Complete", null))
            .ReturnsAsync(JsonConvert.SerializeObject(documentId));

        // Act
        var result = await _service.UploadSimple(jsonData, documentResponse);

        // Assert
        Assert.Equal(documentId, result);
        _httpServiceMock.Verify(x => x.PostAsync(
            It.Is<string>(url => url.Contains("/API/Upload")),
            It.IsAny<HttpContent>(),
            "Initiate"), Times.Once);
        _httpServiceMock.Verify(x => x.GetAsync(
            It.Is<string>(url => url.Contains("file/url")),
            "Upload", null), Times.Once);
        _httpServiceMock.Verify(x => x.GetAsync(
            It.Is<string>(url => url.Contains("complete")),
            "Complete", null), Times.Once);
        _azureBlobServiceMock.Verify(x => x.UploadFile(
            It.IsAny<string>(),
            It.IsAny<byte[]>()), Times.AtLeastOnce);
    }
}
