

namespace Newsletter.Domain.Entities;

public class Subscription
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public string? ExternalSubscriptionId { get; set; }
    public string Provider { get; set; } = "Stripe";

    public string? Plan { get; set; }

    public string Status { get; set; } = "pending";
    
    public DateTime? NextDeliveryDate { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime? CanceledAt { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

