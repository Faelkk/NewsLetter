using Newsletter.Domain.Interfaces;



using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newsletter.Infrastructure.Settings;


namespace Newsletter.Infrastructure.Email;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;
    
    
    public EmailService(IOptions<EmailSettings> options, ILogger<EmailService> logger)
    {
        _emailSettings = options.Value;
        _logger = logger;
    }

    public async Task SendMonthlyEmail(string UserEmail,string body)
    {
        string subject = "Sua newsletter mensal";

        var smtpHost = _emailSettings.SmtpServer;
        var smtpPort = _emailSettings.Port;
        var smtpUser = _emailSettings.From;
        var smtpPass = _emailSettings.Password;
        var fromEmail = _emailSettings.From;


        var client = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(smtpUser, smtpPass),
            EnableSsl = true
        };

        var message = new MailMessage(fromEmail, UserEmail, subject, body);

        try
        {
            await client.SendMailAsync(message);
            _logger.LogInformation($"Email enviado para {UserEmail}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao enviar email para {UserEmail}");
        }
    }
}
