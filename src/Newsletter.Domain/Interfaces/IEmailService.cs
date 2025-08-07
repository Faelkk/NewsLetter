namespace Newsletter.Domain.Interfaces;

public interface IEmailService
{
    Task SendMonthlyEmail(string UserEmail, string body);
}