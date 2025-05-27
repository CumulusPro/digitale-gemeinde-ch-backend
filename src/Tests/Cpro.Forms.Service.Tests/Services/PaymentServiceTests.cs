using Cpro.Forms.Integration.Straatos.Services;
using Cpro.Forms.Service.Models.Payment;
using Cpro.Forms.Service.Services;

namespace Cpro.Forms.Service.Tests.Services;

public class PaymentServiceTests
{
    private readonly Mock<ILogger<HttpService>> _loggerMock;
    private readonly PaymentService _paymentService;

    public PaymentServiceTests()
    {
        _loggerMock = new Mock<ILogger<HttpService>>();
        _paymentService = new PaymentService(_loggerMock.Object);
    }

    [Fact]
    public async Task CreatePaymentRequest_ThrowsException_WhenApiKeyIsInvalid()
    {
        // Arrange
        var request = new PaymentRequest
        {
            amount = "100",
            currency = "USD",
            referenceId = "ref-123",
            formId = "form-1",
            tenantId = 1,
            baseurl = "http://localhost",
            apikey = "api-key",
            instance = "billing"
        };

        // Act and Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _paymentService.CreatePaymentRequest(request));
        Assert.Contains("API secret is not correct", exception.Message);
    }

    [Fact]
    public void BuildSignature_ReturnsExpectedSignature()
    {
        // Arrange
        var query = "amount=100&currency=USD";
        var secret = "my-secret";

        // Act
        var signature = PaymentService.BuildSignature(query, secret);

        // Assert
        Assert.NotNull(signature);
        Assert.IsType<string>(signature);
        Assert.NotEmpty(signature);
    }
}