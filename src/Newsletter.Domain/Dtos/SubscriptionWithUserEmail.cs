namespace Newsletter.Domain.Dtos;


public class SubscriptionWithUserEmail
{
    public Guid SubscriptionId { get; set; }
    public Guid UserId { get; set; }
    public string UserEmail { get; set; }

    // Altere aqui:
    public string[] Interests { get; set; } = [];

    public DateTime? NextDeliveryDate { get; set; }
}


