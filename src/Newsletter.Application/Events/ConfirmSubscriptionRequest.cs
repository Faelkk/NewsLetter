namespace Newsletter.Application.Events;

public class ConfirmSubscriptionRequest
{
    public Guid SubscriptionId { get; set; }
    public string Status { get; set; }
    public string ExternalSubscriptionId { get; set; } = null!;
    
    public string PlanId { get; set; } = null!;
}


