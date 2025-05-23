using Cpro.Forms.Integration.SendGrid.Configuration;
using Cpro.Forms.Integration.SendGrid.Dto;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Logging;

namespace Cpro.Forms.Integration.SendGrid;

public interface ISendGridService
{
    Task SendEmail(EmailDto emailModel);
}