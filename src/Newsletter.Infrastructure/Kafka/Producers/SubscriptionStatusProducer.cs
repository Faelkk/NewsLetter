using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Newsletter.Infrastructure.Stripe;

namespace Newsletter.Infrastructure.Kafka.Producers;

public class SubscriptionStatusProducer : ISubscriptionStatusProducer
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<SubscriptionStatusProducer> _logger;
    private const string Topic = "subscription-status-updated";


    public SubscriptionStatusProducer(ILogger<SubscriptionStatusProducer> logger)
    {
        _logger = logger;

        var config = new ProducerConfig
        {
            BootstrapServers = "broker:9092"
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishAsync(object payload)
    {
        var json = JsonSerializer.Serialize(payload);
        var message = new Message<string, string>
        {
            Key = Guid.NewGuid().ToString(),
            Value = json
        };

        try
        {
            var result = await _producer.ProduceAsync(Topic, message);
            _logger.LogInformation("Mensagem publicada no Kafka: {Offset}", result.Offset);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex, "Erro ao publicar mensagem no Kafka");
            throw;
        }
    }
}
