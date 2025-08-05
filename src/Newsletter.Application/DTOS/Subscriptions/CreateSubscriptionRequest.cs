using System.ComponentModel.DataAnnotations;


namespace Newsletter.Application.DTOS.Subscriptions;

public record CreateSubscriptionRequest(
    [Required] Guid UserId,
    
    [Required(ErrorMessage = "O provedor é obrigatório.")]
    string Provider,
    
    [Required(ErrorMessage = "O plano é obrigatório.")]
    string Plan
);
