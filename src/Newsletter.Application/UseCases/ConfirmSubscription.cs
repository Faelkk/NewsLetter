using Newsletter.Application.Events;
using Newsletter.Application.Interfaces;
using Newsletter.Domain.Interfaces;

namespace Newsletter.Application.UseCases;

public class ConfirmSubscriptionStatus : IConfirmSubscriptionStatus 
{
    private readonly ISubscriptionRepository _subscriptionRepository;

    public ConfirmSubscriptionStatus(ISubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<bool> ExecuteAsync(ConfirmSubscriptionRequest request)
    {

        var subscription = await _subscriptionRepository.GetBySubscriptionIdAsync(request.SubscriptionId);
        if (subscription == null) return false;

        subscription.Status = request.Status;
        subscription.UpdatedAt = DateTime.UtcNow;
        subscription.StartedAt = DateTime.UtcNow;

        await _subscriptionRepository.UpdateAsync(subscription);
        return true;
    }
}
