using Cpro.Forms.Integration.SendGrid.Dto;

namespace Cpro.Forms.Integration.SendGrid;

/// <summary>
/// Interface for SendGrid email service operations.
/// </summary>
public interface ISendGridService
{
    /// <summary>
    /// Sends an email using SendGrid API with HTML content, attachments, and optional placeholders.
    /// </summary>
    /// <param name="emailModel">The email model containing recipient, subject, content, and attachments</param>
    Task SendEmail(EmailDto emailModel);
}