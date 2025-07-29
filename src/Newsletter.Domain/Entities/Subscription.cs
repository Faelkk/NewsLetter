namespace NewsLetter.Domain.Entities;

    


public class Subscription
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public string ExternalSubscriptionId { get; set; } = null!;
    public string Provider { get; set; } = "MercadoPago";

    public string Status { get; set; } = "pending"; 
    public DateTime? StartedAt { get; set; }
    public DateTime? ExpiresAt { get; set; } 
    public DateTime? CanceledAt { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}