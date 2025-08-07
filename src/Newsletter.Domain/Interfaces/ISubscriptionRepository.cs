

using Newsletter.Domain.Entities;
using Newsletter.Domain.Dtos;

namespace Newsletter.Domain.Interfaces;

public interface ISubscriptionRepository
{
    
    Task<IEnumerable<SubscriptionWithUserEmail>> GetActiveSubscriptionsWithUserEmail(DateTime deliveryDate);


    Task<IEnumerable<(Guid Id, DateTime? NextDeliveryDate)>> GetNextDeliveryDatesRawAsync();
    
    Task<IEnumerable<Subscription>> GetAllAsync();
    
    Task<Subscription?> GetByUserIdAsync(Guid userId);
    Task<Subscription?> GetBySubscriptionIdAsync(Guid subscriptionId);

    Task<Subscription?> GetBySubscriptionIdAndUserIdAsync(Guid subscriptionId, Guid userId);

    Task<Subscription> CreateAsync(Subscription subscription);
    
    Task<Subscription?> UpdateAsync(Subscription subscription);
    
    Task SetPendingStatusForExpiredSubscriptionsAsync(DateTime today);

    Task UpdateNextDeliveryDateAsync(Guid id, DateTime? nextDeliveryDate);
    
    Task<bool> DeleteAsync(Guid userId);
}