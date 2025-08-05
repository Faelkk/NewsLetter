using Newsletter.Domain.Entities;

namespace Newsletter.Application.Interfaces;

public interface IGetSubscriptionStatus
{
    Task<Subscription?> ExecuteAsync(Guid subscriptionId);
}