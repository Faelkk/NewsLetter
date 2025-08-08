using Moq;
using Newsletter.Application.Interfaces;
using Newsletter.Application.Jobs;
using Newsletter.Domain.Dtos;
using Newsletter.Domain.Interfaces;

namespace NewsLetter.Test.Jobs;

public class SendMonthlyEmailsJobTests
{
    [Fact]
    public async Task Execute_SendEmails_AndUpdateNextDelivery()
    {
        var mockSubRepo = new Mock<ISubscriptionRepository>();
        var mockEmailService = new Mock<IEmailService>();
        var mockGenerateNewsLetters  = new Mock<IGenerateNewsLetters>();
        
        var subscriptions = new List<(Guid SubscriptionId, Guid UserId, string UserEmail, string[] Interests, DateTime? NextDeliveryDate)>
        {
            (Guid.NewGuid(), Guid.NewGuid(), "test@example.com", ["tech", "news" ], DateTime.UtcNow.Date)
        };

        var subscriptionsWithEmail = new List<SubscriptionWithUserEmail>
        {
            new SubscriptionWithUserEmail
            {
                SubscriptionId = subscriptions[0].SubscriptionId,
                UserId = Guid.NewGuid(),
                Interests =  ["Tech","Books"],
                NextDeliveryDate = DateTime.UtcNow.Date,
                UserEmail = "test@example.com"  
            }
        };

        mockSubRepo.Setup(r => r.GetActiveSubscriptionsWithUserEmail(It.IsAny<DateTime>()))
            .ReturnsAsync(subscriptionsWithEmail);


        mockGenerateNewsLetters.Setup(g => g.GenerateNewsLetterAndSave(It.IsAny<Guid>(), It.IsAny<string>()))
            .ReturnsAsync("Teste body");


        var job = new SendMonthlyEmailsJob(mockSubRepo.Object, mockEmailService.Object, mockGenerateNewsLetters.Object);

        await job.Execute();

        mockGenerateNewsLetters.Setup(g => g.GenerateNewsLetterAndSave(It.IsAny<Guid>(), It.IsAny<string>()))
            .ReturnsAsync("Teste body");

        mockEmailService.Verify(e => e.SendMonthlyEmail("test@example.com", "Teste body"), Times.Once);
        mockSubRepo.Verify(r => r.UpdateNextDeliveryDateAsync(It.IsAny<Guid>(), It.IsAny<DateTime?>()), Times.Once);

    }
}