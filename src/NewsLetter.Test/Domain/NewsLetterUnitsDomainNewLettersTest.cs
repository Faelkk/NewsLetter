using Newsletter.Domain.Entities;

namespace NewsLetter.Test.Domain;

public class NewsLetterUnitsDomainNewLettersTest
{
    [Fact]
    public void Should_Create_NewLetters_With_Valid_Data()
    {
        var now = DateTime.Now;

        var user = new NewsletterEntry()
        {
            Id = Guid.NewGuid(),
            Topics = ["Tecnologia"],
            UserId = Guid.NewGuid(),
            Content = "Testando conteudo",
            CreatedAt = now,
        };

        Assert.NotNull(user);
        Assert.Equal(now, user.CreatedAt); 
        Assert.Contains("Tecnologia", user.Topics);
        Assert.Contains("Testando conteudo", user.Content);
    }

}