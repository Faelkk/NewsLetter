using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newsletter.Application.Events;
using Newsletter.Worker.Handlers;

namespace Newsletter.Worker.Consumers;

public class SubscriptionStatusConsumer : BackgroundService
{
    private readonly ILogger<SubscriptionStatusConsumer> _logger;
    private readonly IConsumer<string,string> _consumer;
    private readonly SubscriptionStatusUpdateHandler _handler;

    public SubscriptionStatusConsumer(ILogger<SubscriptionStatusConsumer> logger,SubscriptionStatusUpdateHandler handler)
    {
        _logger = logger;
        _handler = handler;
        var config =  new ConsumerConfig
        {
            BootstrapServers = "broker:9092", 
            GroupId = "newsletter-subscription-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        
        _consumer = new ConsumerBuilder<string, string>(config).Build();
        _consumer.Subscribe("subscription-status-updated");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Subscription status consumer is running.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = _consumer.Consume(stoppingToken);
                
                _logger.LogInformation("Valor recebido no Kafka: {Value}", result.Message.Value);

                _logger.LogInformation($"Consumed new subscription status: {result.Value}");
        
                 var data = JsonSerializer.Deserialize<SubscriptionStatusUpdatedEvent>(result.Message.Value);
                 Console.WriteLine(("Caiu no consumer"));
                 await _handler.HandleAsync(data);
                 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }

    public override void Dispose()  
    {
        _consumer.Close();
        _consumer.Dispose();
        base.Dispose();
    }
}


