using FluentAssertions;
using Newsletter.Domain.Entities;
using Newsletter.Infrastructure.Services;
using System.Threading.Tasks;
using NewsLetter.Test.Test.Services;
using Xunit;

namespace Newsletter.Tests.Integration.Services;

public class EmailServiceIntegrationTests
{
    private readonly FakeEmailService _emailService;

    public EmailServiceIntegrationTests()
    {
        _emailService = new FakeEmailService();
    }

    [Fact]
    public async Task SendMonthlyEmail_ShouldStoreEmailInSentList()
    {
        var body = "Conteúdo do newsletter de teste";
        await _emailService.SendMonthlyEmail("user@example.com", body);

        Assert.Single(_emailService.SentEmails);
        Assert.Equal("user@example.com", _emailService.SentEmails[0].Recipient);
        Assert.Equal(body, _emailService.SentEmails[0].Body);
    }

}