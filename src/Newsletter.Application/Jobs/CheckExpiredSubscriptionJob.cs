using Newsletter.Application.Jobs.Interfaces;
using Newsletter.Domain.Interfaces;

public class CheckExpiredSubscriptionJob : ICheckExpiredSubscriptionJob
{
    private readonly ISubscriptionRepository _subscriptionRepository;

    public CheckExpiredSubscriptionJob(ISubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task Execute()
    {
        var today = DateTime.UtcNow.Date;
        await _subscriptionRepository.SetPendingStatusForExpiredSubscriptionsAsync(today);
    }
}