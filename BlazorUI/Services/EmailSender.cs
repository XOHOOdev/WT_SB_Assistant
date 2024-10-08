using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid;
using SendGrid.Helpers.Mail;
using Sparta.Core.Helpers;

namespace Sparta.BlazorUI.Services;

public class EmailSender(ILogger<EmailSender> logger, ConfigHelper config) : IEmailSender
{
    private readonly ILogger _logger = logger;
    private readonly string _sendGridKey = config.GetConfig("SendGrid", "SendGridKey") ?? string.Empty;

    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        if (string.IsNullOrEmpty(_sendGridKey)) throw new Exception("Null SendGridKey");
        await Execute(_sendGridKey, subject, message, toEmail);
    }

    public async Task Execute(string apiKey, string subject, string message, string toEmail)
    {
        var client = new SendGridClient(apiKey);
        var msg = new SendGridMessage
        {
            From = new EmailAddress("Spartadcbot@gmail.com", "Sparta"),
            Subject = subject,
            PlainTextContent = message,
            HtmlContent = message
        };
        msg.AddTo(new EmailAddress(toEmail));

        // Disable click tracking.
        // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
        msg.SetClickTracking(false, false);
        var response = await client.SendEmailAsync(msg);
        _logger.LogInformation(response.IsSuccessStatusCode
            ? $"Email to {toEmail} queued successfully!"
            : $"Failure Email to {toEmail}");
    }
}