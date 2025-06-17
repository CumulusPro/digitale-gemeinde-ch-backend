using Cpro.Forms.Integration.SendGrid.Configuration;
using Cpro.Forms.Integration.SendGrid.Dto;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Cpro.Forms.Integration.SendGrid;

/// <inheritdoc />
public class SendGridService : ISendGridService
{
    private readonly ISendGridConfig _configuration;
    private readonly ILogger<SendGridService> _logger;

    public SendGridService(ISendGridConfig sendGridConfig, ILogger<SendGridService> logger)
    {
        _configuration = sendGridConfig;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task SendEmail(EmailDto emailModel)
    {
        var apiKey = _configuration.APIKey;
        var client = new SendGridClient(apiKey);
        
        // Initialize SendGridMessage
        var sendGridMessage = new SendGridMessage();
        sendGridMessage.SetFrom(_configuration.FromEmail, _configuration.Name);
        
        // Add recipients
        sendGridMessage.AddTo(emailModel.ToEmail);
       
        // Set email subject
        sendGridMessage.SetSubject(emailModel.Subject ?? "No Subject");

        // Set HTML content from the EmailRequestModel
        sendGridMessage.HtmlContent = emailModel.HtmlContent;
        
        // Add placeholders if applicable
        if (emailModel.PlaceHolders != null && emailModel.PlaceHolders.Any())
        {
            // string templateBody = JsonConvert.SerializeObject(emailModel.PlaceHolders, Formatting.Indented);
            // sendGridMessage.SetTemplateData(JObject.Parse(templateBody));
        }

        // Add attachments
        if (emailModel.AttachmentPlaceHolders?.Count > 0)
        {
            foreach (var atch in emailModel.AttachmentPlaceHolders)
            {
                var mailAttachment = new Attachment
                {
                    Content = Convert.ToBase64String(atch.Key),  // Ensure atch.Key is byte data
                    Filename = atch.Value,
                    Type = "application/octet-stream"  // Specify MIME type if known
                };
                sendGridMessage.AddAttachment(mailAttachment);
            }
        }

        // Send email and log the response
        var response = await client.SendEmailAsync(sendGridMessage);
        _logger.LogDebug(sendGridMessage.Serialize());
        _logger.LogInformation($"Email Response Status: {response.StatusCode}");
    }
}
