using Stripe;
using Microsoft.Extensions.Logging;
using Newsletter.Infrastructure.Kafka.Producers;

namespace Newsletter.Infrastructure.Stripe;


public class StripeWebhookHandler
{
    private readonly SubscriptionStatusProducer _producer;
    private readonly ILogger<StripeWebhookHandler> _logger;

    public StripeWebhookHandler(SubscriptionStatusProducer producer, ILogger<StripeWebhookHandler> logger)
    {
        _producer = producer;
        _logger = logger;
    }

    public async Task HandleAsync(Event stripeEvent)
    {
        _logger.LogInformation("Evento Stripe recebido: {Type}", stripeEvent.Type);

        switch (stripeEvent.Type)
        {
            case "customer.subscription.deleted":
            {
                var subscription = stripeEvent.Data.Object as Subscription;

                var payload = new
                {
                    EventType = stripeEvent.Type,
                    SubscriptionId = subscription?.Id,
                    CustomerId = subscription?.CustomerId,
                    Timestamp = stripeEvent.Created
                };

                await _producer.PublishAsync(payload);

                break;
            }

            case "invoice.paid":
            {
                var invoice = stripeEvent.Data.Object as Invoice;

                var payload = new
                {
                    EventType = stripeEvent.Type,
                    InvoiceId = invoice?.Id,
                    CustomerId = invoice?.CustomerId,
                    Timestamp = stripeEvent.Created
                };

                await _producer.PublishAsync(payload);

                break;
            }

            default:
                _logger.LogWarning("Evento Stripe não tratado: {Type}", stripeEvent.Type);
                break;
        }
    }
}