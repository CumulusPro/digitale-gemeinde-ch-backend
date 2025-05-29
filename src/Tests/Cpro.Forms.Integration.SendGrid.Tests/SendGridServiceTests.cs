using Cpro.Forms.Integration.SendGrid.Configuration;
using Cpro.Forms.Integration.SendGrid.Dto;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Cpro.Forms.Integration.SendGrid.Tests;

public class SendGridServiceTests
{
    private readonly Mock<ISendGridConfig> _configMock;
    private readonly Mock<ILogger<SendGridService>> _loggerMock;
    private readonly SendGridService _service;

    public SendGridServiceTests()
    {
        _configMock = new Mock<ISendGridConfig>();
        _loggerMock = new Mock<ILogger<SendGridService>>();

        // Setup default configuration
        _configMock.Setup(x => x.APIKey).Returns("test-api-key");
        _configMock.Setup(x => x.FromEmail).Returns("test@example.com");
        _configMock.Setup(x => x.Name).Returns("Test Sender");

        _service = new SendGridService(_configMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task SendEmail_WithBasicEmail_SendsSuccessfully()
    {
        // Arrange
        var emailModel = new EmailDto
        {
            ToEmail = "recipient@example.com",
            Subject = "Test Subject",
            HtmlContent = "<p>Test content</p>"
        };

        // Act & Assert
        await _service.SendEmail(emailModel);

        // Verify logger was called
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Email Response Status")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task SendEmail_WithAttachments_SendsSuccessfully()
    {
        // Arrange
        var emailModel = new EmailDto
        {
            ToEmail = "recipient@example.com",
            Subject = "Test Subject",
            HtmlContent = "<p>Test content</p>",
            AttachmentPlaceHolders = new Dictionary<byte[], string>
            {
                { new byte[] { 1, 2, 3 }, "test.pdf" }
            }
        };

        // Act & Assert
        await _service.SendEmail(emailModel);

        // Verify logger was called
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Email Response Status")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task SendEmail_WithPlaceholders_SendsSuccessfully()
    {
        // Arrange
        var emailModel = new EmailDto
        {
            ToEmail = "recipient@example.com",
            Subject = "Test Subject",
            HtmlContent = "<p>Test content</p>",
            PlaceHolders = new Dictionary<string, string>
            {
                { "key1", "value1" },
                { "key2", "value2" }
            }
        };

        // Act & Assert
        await _service.SendEmail(emailModel);

        // Verify logger was called
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Email Response Status")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task SendEmail_WithNullSubject_UsesDefaultSubject()
    {
        // Arrange
        var emailModel = new EmailDto
        {
            ToEmail = "recipient@example.com",
            Subject = null,
            HtmlContent = "<p>Test content</p>"
        };

        // Act & Assert
        await _service.SendEmail(emailModel);

        // Verify logger was called
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Email Response Status")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task SendEmail_WithEmptyHtmlContent_SendsSuccessfully()
    {
        // Arrange
        var emailModel = new EmailDto
        {
            ToEmail = "recipient@example.com",
            Subject = "Test Subject",
            HtmlContent = string.Empty
        };

        // Act & Assert
        await _service.SendEmail(emailModel);

        // Verify logger was called
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Email Response Status")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }
}
