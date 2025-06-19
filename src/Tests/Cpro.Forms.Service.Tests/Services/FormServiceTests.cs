using Cpro.Forms.Data.Repositories;
using Cpro.Forms.Integration.SendGrid;
using Cpro.Forms.Integration.Storage.Services;
using Cpro.Forms.Integration.Straatos.Services;
using Cpro.Forms.Service.Configuration;
using Cpro.Forms.Service.Models;
using Cpro.Forms.Service.Services;
using Newtonsoft.Json;
using Peritos.Common.Abstractions;
using Peritos.Common.Abstractions.Paging;
using System.Dynamic;

namespace Cpro.Forms.Service.Tests.Services;

public class FormServiceTests
{
    private readonly Mock<IStraatosApiService> _straatosApiServiceMock;
    private readonly Mock<IAzureBlobService> _azureBlobServiceMock;
    private readonly Mock<IFormDesignerService> _formDesignerServiceMock;
    private readonly Mock<IPaymentService> _paymentServiceMock;
    private readonly Mock<ISendGridService> _sendgridServiceMock;
    private readonly Mock<IFormDataRepository> _formDataRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRequestContext> _requestContextMock;
    private readonly Mock<IServiceConfig> _serviceConfigMock;
    private readonly FormService _formService;

    public FormServiceTests()
    {
        _straatosApiServiceMock = new Mock<IStraatosApiService>();
        _azureBlobServiceMock = new Mock<IAzureBlobService>();
        _formDesignerServiceMock = new Mock<IFormDesignerService>();
        _paymentServiceMock = new Mock<IPaymentService>();
        _sendgridServiceMock = new Mock<ISendGridService>();
        _formDataRepositoryMock = new Mock<IFormDataRepository>();
        _mapperMock = new Mock<IMapper>();
        _requestContextMock = new Mock<IRequestContext>();
        _serviceConfigMock = new Mock<IServiceConfig>();

        _formService = new FormService(
            _straatosApiServiceMock.Object,
            _azureBlobServiceMock.Object,
            _formDataRepositoryMock.Object,
            _paymentServiceMock.Object,
            _mapperMock.Object,
            _sendgridServiceMock.Object,
            _requestContextMock.Object,
            _formDesignerServiceMock.Object,
            _serviceConfigMock.Object);
    }

    [Fact]
    public async Task GetFormDataAsync_ReturnsDocumentResponse_WhenDocumentIdExists()
    {
        // Arrange
        var formId = "test-form-id";
        var tenantId = 1;
        var documentId = "test-document-id";
        var formData = new Data.Models.FormData
        {
            Status = "Completed"
        };
        var documentJson = JsonConvert.SerializeObject(new DocumentResponse { Fields = new List<IndexField>() });
        var expectedResponse = new DocumentResponse
        {
            header = new Header { showHeader = false },
            footer = new Footer { showFooter = false },
            ShowSubmit = false,
            State = formData.Status
        };

        _formDataRepositoryMock.Setup(x => x.GetFormDataByDocumentId(documentId))
            .ReturnsAsync(formData);
        _azureBlobServiceMock.Setup(x => x.GetFile($"{documentId}.json"))
            .ReturnsAsync(documentJson);

        // Act
        var result = await _formService.GetFormDataAsync(formId, tenantId, documentId);

        // Assert
        Assert.Equal(expectedResponse.header.showHeader, result.header.showHeader);
        Assert.Equal(expectedResponse.footer.showFooter, result.footer.showFooter);
        Assert.Equal(expectedResponse.ShowSubmit, result.ShowSubmit);
        Assert.Equal(expectedResponse.State, result.State);
        _formDataRepositoryMock.Verify(x => x.GetFormDataByDocumentId(documentId), Times.Once);
        _azureBlobServiceMock.Verify(x => x.GetFile($"{documentId}.json"), Times.Once);
    }

    [Fact]
    public async Task GetFormDataAsync_ReturnsFormDefinition_WhenDocumentIdDoesNotExist()
    {
        // Arrange
        var formId = "test-form-id";
        var tenantId = 1;
        var documentId = string.Empty;
        var expectedResponse = new DocumentResponse
        {
            designers = new List<string> { "test@example.com" }
        };

        _requestContextMock.Setup(x => x.UserEmail)
            .Returns("test@example.com");
        _requestContextMock.Setup(x => x.UserId)
            .Returns(1);
        _formDesignerServiceMock.Setup(x => x.GetFormDefinitionResponseAsync(formId, tenantId))
            .ReturnsAsync(expectedResponse);
        _serviceConfigMock.Setup(x => x.UseStraatos).Returns(false);

        // Act
        var result = await _formService.GetFormDataAsync(formId, tenantId, documentId);

        // Assert
        Assert.Equal(expectedResponse, result);
        Assert.False(result.isFormConfigDisabled);
        _formDesignerServiceMock.Verify(x => x.GetFormDefinitionResponseAsync(formId, tenantId), Times.Once);
    }

    [Fact]
    public async Task GetFormNavigationAsync_ReturnsNavigationList_WhenNavigationExists()
    {
        // Arrange
        var tenantId = 1;
        var formDesigns = new List<FormDesign>
        {
            new FormDesign { id = "form1", Name = "Form 1", FormId = 1 },
            new FormDesign { id = "form2", Name = "Form 2", FormId = 2 }
        };
        var formDatas = new List<Data.Models.FormData>
        {
            new Data.Models.FormData { FormId = "form1", Status = "Completed" },
            new Data.Models.FormData { FormId = "form1", Status = "Pending" },
            new Data.Models.FormData { FormId = "form2", Status = "Completed" }
        };

        _formDesignerServiceMock.Setup(x => x.GetFormDesignsByTenantIdAsync(tenantId))
            .ReturnsAsync(formDesigns);
        _formDataRepositoryMock.Setup(x => x.GetFormDatasByTenantId(tenantId))
            .ReturnsAsync(formDatas);

        // Act
        var result = await _formService.GetFormNavigationAsync(tenantId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        var form1Nav = result.FirstOrDefault(f => f.Id == "form1");
        Assert.NotNull(form1Nav);
        Assert.Equal("Form 1", form1Nav.Name);
        Assert.Equal(2, form1Nav.Count); // 2 statuses for form1

        var form2Nav = result.FirstOrDefault(f => f.Id == "form2");
        Assert.NotNull(form2Nav);
        Assert.Equal("Form 2", form2Nav.Name);
        Assert.Equal(1, form2Nav.Count); // 1 status for form2

        _formDesignerServiceMock.Verify(x => x.GetFormDesignsByTenantIdAsync(tenantId), Times.Once);
        _formDataRepositoryMock.Verify(x => x.GetFormDatasByTenantId(tenantId), Times.Once);
    }

    [Fact]
    public async Task SearchFormData_ReturnsPagingResponse_WhenFormsFound()
    {
        // Arrange
        var searchRequest = new FormSearchRequest { Keyword = "Form", FormId = "1", Status = "Completed", Page = 1, PageSize = 10 };
        var tenantId = 1;
        var expectedData = new List<FormData>
        {
            new FormData { FormId = "1", Name = "Form 1", Status = "Completed" }
        };
        var expectedResponse = new PagingResponse<FormData> { Data = expectedData };

        _mapperMock.Setup(x => x.Map<Data.Models.FormSearchRequest>(searchRequest))
            .Returns(new Data.Models.FormSearchRequest());
        _formDataRepositoryMock.Setup(x => x.SearchFormDatasAsync(It.IsAny<Data.Models.FormSearchRequest>(), tenantId))
            .ReturnsAsync(new PagingResponse<Data.Models.FormData>());
        _mapperMock.Setup(x => x.Map<PagingResponse<FormData>>(It.IsAny<PagingResponse<Data.Models.FormData>>()))
            .Returns(expectedResponse);

        // Act
        var result = await _formService.SearchFormData(searchRequest, tenantId);

        // Assert
        Assert.Equal(expectedResponse, result);
        _mapperMock.Verify(x => x.Map<Data.Models.FormSearchRequest>(searchRequest), Times.Once);
        _formDataRepositoryMock.Verify(x => x.SearchFormDatasAsync(It.IsAny<Data.Models.FormSearchRequest>(), tenantId), Times.Once);
        _mapperMock.Verify(x => x.Map<PagingResponse<FormData>>(It.IsAny<PagingResponse<Data.Models.FormData>>()), Times.Once);
    }

    [Fact]
    public async Task SubmitTaskAsync_PaymentDisabled_SendsEmailAndSavesFormData()
    {
        // Arrange
        dynamic jsonData = new ExpandoObject();
        jsonData.formId = "form1";
        jsonData.tenantId = 1;
        jsonData.data = new ExpandoObject();
        jsonData.data.FormId = jsonData.formId;

        var documentResponse = new DocumentResponse
        {
            Name = "Test Form",
            paymentConfig = new PaymentConfig { isEnabled = 0 },
            emailNotificationConfig = new EmailNotificationConfig
            {
                notificationEnabled = true,
                emailsTo = "test@example.com",
                emailsSubject = "Subject",
                emailsBody = "Body documenturl",
                baseurl = "http://baseurl"
            }
        };

        _formDesignerServiceMock
            .Setup(x => x.GetFormDefinitionResponseAsync("form1", 1))
            .ReturnsAsync(documentResponse);

        _serviceConfigMock.Setup(x => x.UseStraatos).Returns(false);
        _formDataRepositoryMock.Setup(x => x.GetNextSequenceDocumentId())
            .ReturnsAsync("doc123");

        _sendgridServiceMock.Setup(x => x.SendEmail(It.IsAny<Integration.SendGrid.Dto.EmailDto>()))
            .Returns(Task.CompletedTask);

        _formDataRepositoryMock.Setup(x => x.CreateFormDataAsync(It.IsAny<Data.Models.FormData>()))
            .ReturnsAsync(new Data.Models.FormData());

        // Act
        var result = await _formService.SubmitTaskAsync(jsonData, "http://origin");

        // Assert
        Assert.Equal("doc123", result.documentId);
        Assert.Null(result.redirectUrl);
        _sendgridServiceMock.Verify(x => x.SendEmail(It.IsAny<Integration.SendGrid.Dto.EmailDto>()), Times.Once);
        _formDataRepositoryMock.Verify(x => x.CreateFormDataAsync(It.IsAny<Data.Models.FormData>()), Times.Once);
    }

    [Fact]
    public async Task GetCurrentUserDetails_ReturnsCurrentUser_FromStraatos()
    {
        // Arrange
        _serviceConfigMock.Setup(x => x.UseStraatos).Returns(true);
        _requestContextMock.SetupGet(x => x.Token).Returns("token");

        var userJson = JsonConvert.SerializeObject(new CurrentUser { Id = 1, Emails = new List<string> { "test@example.com" } });
        _straatosApiServiceMock.Setup(x => x.GetCurrentUser("token")).ReturnsAsync(userJson);

        // Act
        var result = await InvokePrivateMethodAsync<CurrentUser>(_formService, "GetCurrentUserDetails");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Contains("test@example.com", result.Emails);
    }

    [Fact]
    public async Task GetCurrentUserDetails_ReturnsCurrentUser_FromRequestContext()
    {
        // Arrange
        _serviceConfigMock.Setup(x => x.UseStraatos).Returns(false);
        _requestContextMock.Setup(x => x.UserId).Returns(5);
        _requestContextMock.Setup(x => x.UserEmail).Returns("user@example.com");

        // Act
        var result = await InvokePrivateMethodAsync<CurrentUser>(_formService, "GetCurrentUserDetails");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Id);
        Assert.Contains("user@example.com", result.Emails);
    }

    // Helper to invoke private async methods
    private static async Task<T> InvokePrivateMethodAsync<T>(object instance, string methodName, params object[] parameters)
    {
        var method = instance.GetType()
            .GetMethod(methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var task = (Task<T>)method.Invoke(instance, parameters);
        return await task;
    }

}