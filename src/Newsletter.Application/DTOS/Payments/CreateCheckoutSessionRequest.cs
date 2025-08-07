using System.ComponentModel.DataAnnotations;

namespace Newsletter.Application.DTOS.Payments;

public record CreateCheckoutSessionRequest(
    [Required(ErrorMessage = "UserId is required.")]
    Guid UserId,

    [Required(ErrorMessage = "PlanId is required.")]
    string PlanId,

    [Required(ErrorMessage = "SuccessUrl is required.")]
    [Url(ErrorMessage = "SuccessUrl must be a valid URL.")]
    string SuccessUrl,

    [Required(ErrorMessage = "CancelUrl is required.")]
    [Url(ErrorMessage = "CancelUrl must be a valid URL.")]
    string CancelUrl,
    
    [Required(ErrorMessage = "Subscription id is required.")]
    Guid SubscriptionId 
); 