namespace Newsletter.Infrastructure.Interfaces;

public interface IStripeSubscriptionService
{
    Task CancelSubscriptionAsync(string externalSubscriptionId);
}