using Newsletter.Application.Interfaces;
using Newsletter.Domain.Entities;
using Newsletter.Domain.Interfaces;

namespace Newsletter.Application.UseCases;

public class GetSubscriptionStatus : IGetSubscriptionStatus
{
    private readonly ISubscriptionRepository _subscriptionRepository;

    public GetSubscriptionStatus(ISubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<Subscription?> ExecuteAsync(Guid subscriptionId)
    {
        return await _subscriptionRepository.GetBySubscriptionIdAsync(subscriptionId);
    }
}
