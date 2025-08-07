using Newsletter.Domain.Entities;

namespace NewsLetter.Test.Domain;

public class NewsLetterUnitsDomainSubscriptionTest
{
    [Fact]
    public void Should_Create_NewLetters_With_Valid_Data()
    {
        var now = DateTime.Now;
        var nextMonth = now.AddMonths(1);
        var externalId = Guid.NewGuid().ToString();

        var user = new Subscription()
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            CanceledAt = now,
            ExpiresAt = now,
            ExternalSubscriptionId = externalId,
            NextDeliveryDate = nextMonth,
            Plan = Guid.NewGuid().ToString(),
            Provider = "Stripe",
            StartedAt = now,
            Status = "Active",
            UpdatedAt = now,
        };
        
        Assert.NotNull(user);
        Assert.Equal(now, user.StartedAt);
        Assert.Equal(now, user.ExpiresAt);
        Assert.Equal(externalId, user.ExternalSubscriptionId);
        Assert.Equal(nextMonth, user.NextDeliveryDate);
        Assert.Equal("Stripe", user.Provider);
        Assert.Equal("Active", user.Status);
        Assert.Equal(now, user.UpdatedAt);
    }

}