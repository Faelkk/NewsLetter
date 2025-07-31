using System.ComponentModel.DataAnnotations;
using Newsletter.Domain.Enums;

namespace Newsletter.Application.DTOS.Subscriptions;

public record CreateSubscriptionRequest(
    [Required] Guid UserId,

    [Required(ErrorMessage = "O ID da assinatura externa é obrigatório.")]
    string ExternalSubscriptionId,

    [Required(ErrorMessage = "O provedor é obrigatório.")]
    string Provider,
    
    [Required(ErrorMessage = "O plano é obrigatório.")]
    SubscriptionPlan Plan
);
