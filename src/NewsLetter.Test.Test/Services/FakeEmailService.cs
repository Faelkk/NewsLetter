using Newsletter.Domain.Entities;
using Newsletter.Domain.Interfaces;

namespace NewsLetter.Test.Test.Services;

using System.Collections.Generic;
using System.Threading.Tasks;


public class FakeEmailService : IEmailService
{
    public List<SentEmail> SentEmails { get; } = new();

    public Task SendMonthlyEmail(string userEmail, string body)
    {
        SentEmails.Add(new SentEmail
        {
            Recipient = userEmail,
            Body = body
        });

        return Task.CompletedTask;
    }
}

public class SentEmail
{
    public string Recipient { get; set; } = null!;
    public string Body { get; set; } = null!;
}
