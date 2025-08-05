using Stripe;
using Stripe.Checkout;

using Microsoft.Extensions.Logging;
using Newsletter.Infrastructure.Kafka.Producers;

namespace Newsletter.Infrastructure.Stripe
{
    public class StripeWebhookHandler : IStripeWebhookHandler
    {
        private readonly ISubscriptionStatusProducer _producer;
        private readonly ILogger<StripeWebhookHandler> _logger;

        public StripeWebhookHandler(ISubscriptionStatusProducer producer, ILogger<StripeWebhookHandler> logger)
        {
            _producer = producer;
            _logger = logger;
        }

        public async Task HandleAsync(Event stripeEvent)
        {
            _logger.LogInformation("Evento Stripe recebido: {Type}", stripeEvent.Type);

            switch (stripeEvent.Type)
            {
                case "checkout.session.completed":
                {
                    var sessionService = new SessionService();

                    var session = await sessionService.GetAsync(
                        ((Session)stripeEvent.Data.Object).Id,
                        new SessionGetOptions
                        {
                            Expand = new List<string> { "subscription" }
                        });
                    
                    Console.WriteLine($"Session: {session}");

                    var subscriptionId = session.Subscription?.Id;

                    if (subscriptionId == null)
                    {
                        Console.WriteLine("Subscription ID não encontrado na session");
                        throw new ArgumentNullException("Subscription ID não encontrado na session");
                    }

                    var subscriptionService = new SubscriptionService();
                    var fullSubscription = await subscriptionService.GetAsync(subscriptionId);

                    Console.WriteLine($"Subscription ID: {fullSubscription.Id}");
                    Console.WriteLine($"Subscription Metadata: {System.Text.Json.JsonSerializer.Serialize(fullSubscription.Metadata)}");

                    var metadata = fullSubscription.Metadata;

                    if (metadata != null && metadata.ContainsKey("subscription_id"))
                    {
                        var subscriptionIdMetaData = metadata["subscription_id"];

                        var payload = new
                        {
                            SubscriptionId = subscriptionId,
                            Status = "Active",
                            ExternalSubscriptionId = subscriptionIdMetaData
                        };

                        await _producer.PublishAsync(payload);
                    }
                    else
                    {
                        Console.WriteLine("Metadata 'subscription_id' não encontrada na subscription");
                        throw new ArgumentNullException("Metadata 'subscription_id' não encontrada na subscription");
                    }

                    break;
                }

                default:
                    _logger.LogWarning("Evento Stripe não tratado: {Type}", stripeEvent.Type);
                    break;
            }
        }
    }
}
