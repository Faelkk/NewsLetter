using Newsletter.Application.DTOS.Subscriptions;
using Newsletter.Presentation.DTOS;

namespace Newsletter.Application.Interfaces;

public interface ISubscriptionService
{
    Task<SubscriptionDto> GetByUserIdAsync(Guid userId);
    Task<SubscriptionDto> GetBySubscriptionIdAsync(Guid userId, Guid subscriptionId);
    Task<SubscriptionDto> CreateAsync(CreateSubscriptionRequest request);
    Task<SubscriptionDto> UpdateAsync(UpdateSubscriptionRequest request,Guid id);
    Task<bool> DeleteByUserAsync(Guid userId);
}

