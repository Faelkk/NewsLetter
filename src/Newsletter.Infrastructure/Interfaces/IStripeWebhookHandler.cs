using Stripe;


namespace Newsletter.Infrastructure.Stripe;

public interface IStripeWebhookHandler
{
    Task HandleAsync(Event stripeEvent);
}