namespace Newsletter.Domain.Dtos;

public class SubscriptionExpiryDto
{
    public Guid SubscriptionId { get; set; }
    public DateTime ExpiryDate { get; set; }
}