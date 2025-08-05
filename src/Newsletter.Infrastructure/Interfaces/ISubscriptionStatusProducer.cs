namespace Newsletter.Infrastructure.Stripe;

public interface ISubscriptionStatusProducer
{
    Task PublishAsync(object payload);
}