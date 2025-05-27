using Cpro.Forms.Api.Controllers;
using Cpro.Forms.Service.Models.Payment;
using Cpro.Forms.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Cpro.Forms.Api.Tests.Controllers;

public class PaymentControllerTests
{
    private readonly Mock<IPaymentService> _paymentServiceMock;
    private readonly PaymentController _controller;

    public PaymentControllerTests()
    {
        _paymentServiceMock = new Mock<IPaymentService>();
        _controller = new PaymentController(_paymentServiceMock.Object);
    }

    [Fact]
    public async Task SubmitForm_ReturnsOkResult_WhenPaymentSubmitted()
    {
        // Arrange
        var request = new PaymentRequest();
        var expectedResponse = string.Empty;
        _paymentServiceMock.Setup(x => x.CreatePaymentRequest(request))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.SubmitForm(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<string>(okResult.Value);
        Assert.Equal(expectedResponse, returnValue);
    }
} 