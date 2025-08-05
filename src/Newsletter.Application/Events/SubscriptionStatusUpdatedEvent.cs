namespace Newsletter.Application.Events;


public class SubscriptionStatusUpdatedEvent
{
    public Guid SubscriptionId { get; set; }
    public string Status { get; set; }
    public string ExternalSubscriptionId { get; set; } = null!;
}
