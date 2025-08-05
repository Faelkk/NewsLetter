using Microsoft.Extensions.Logging;
using Newsletter.Application.Events;
using Newsletter.Application.Interfaces;
using Newsletter.Application.UseCases;

namespace Newsletter.Worker.Handlers;

public class SubscriptionStatusUpdateHandler
{
    private readonly IConfirmSubscriptionStatus _useCase;
    private readonly ILogger<SubscriptionStatusUpdateHandler> _logger;
    
    
    public SubscriptionStatusUpdateHandler(
        IConfirmSubscriptionStatus useCase,
        ILogger<SubscriptionStatusUpdateHandler> logger)
    {
        _useCase = useCase;
        _logger = logger;
    }
    
    public async Task HandleAsync(SubscriptionStatusUpdatedEvent @event)
{
    _logger.LogInformation("Processing event for SubscriptionId {Id}", @event.SubscriptionId);
    _logger.LogInformation("Payload recebido: {@event}");
    Console.WriteLine("entrou no handle aq do kafka");
    var success = await _useCase.ExecuteAsync(new ConfirmSubscriptionRequest
    {
        SubscriptionId = @event.SubscriptionId,
        Status = @event.Status,
        ExternalSubscriptionId = @event.ExternalSubscriptionId,
        PlanId =  @event.PlanId
        
    });

    if (success)
        _logger.LogInformation("Subscription status updated successfully");
    else
        _logger.LogWarning("Failed to update subscription status");
}

}





