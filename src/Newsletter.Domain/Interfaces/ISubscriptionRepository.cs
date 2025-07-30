

using Newsletter.Domain.Entities;

namespace Newsletter.Domain.Interfaces;

public interface ISubscriptionRepository
{
    Task<IEnumerable<Subscription>> GetAllAsync();
    
    Task<Subscription?> GetByUserIdAsync(Guid userId);

    Task<Subscription> CreateAsync(Subscription subscription);
    
    Task<Subscription?> UpdateAsync(Subscription subscription);
    
    Task<bool> DeleteAsync(Guid userId);
}