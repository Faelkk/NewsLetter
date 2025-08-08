using Moq;
using Newsletter.Domain.Interfaces;
using Xunit;

namespace NewsLetter.Test.Application
{
    public class CheckExpiredSubscriptionJobTests 
    {
        [Fact]
        public async Task Execute_Calls_SetPendingStatusForExpiredSubscriptionsAsync_With_Today()
        {
            var mockSubRepo = new Mock<ISubscriptionRepository>();
            var job = new CheckExpiredSubscriptionJob(mockSubRepo.Object);

            await job.Execute();

            mockSubRepo.Verify(r => r.SetPendingStatusForExpiredSubscriptionsAsync(It.Is<DateTime>(d => d == DateTime.UtcNow.Date)), Times.Once);
        }
    }
}