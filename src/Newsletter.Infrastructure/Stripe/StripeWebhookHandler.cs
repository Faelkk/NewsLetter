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
                    

                    var subscriptionId = session.Subscription?.Id;

                    if (subscriptionId == null)
                    {
                        throw new ArgumentNullException("Subscription ID não encontrado na session");
                    }

                    var subscriptionService = new SubscriptionService();
                    var fullSubscription = await subscriptionService.GetAsync(subscriptionId);
                    var priceId = fullSubscription.Items.Data.FirstOrDefault()?.Price.Id;
                    

                    var metadata = fullSubscription.Metadata;

                    if (metadata != null && metadata.ContainsKey("subscription_id"))
                    {
                        var subscriptionIdMetaData = metadata["subscription_id"];

                        var payload = new
                        {
                            SubscriptionId = subscriptionIdMetaData,
                            Status = "Active",
                            ExternalSubscriptionId = subscriptionId,
                            PlanId = priceId
                        };
                        
                        Console.WriteLine("entrou aq e deu publish");

                        await _producer.PublishAsync(payload);
                    }
                    else
                    {
                        throw new ArgumentNullException("Metadata 'subscription_id' não encontrada na subscription");
                    }

                    break;
                }

                default:
                    break;
            }
        }
    }
}
