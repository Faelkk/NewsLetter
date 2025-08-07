

namespace Newsletter.Presentation.DTOS;

public record UpdateSubscriptionRequest(
    string? ExternalSubscriptionId,
    string? Provider,
    string? Status,
    string? Plan,
    DateTime? NextDeliveryDate,
    DateTime? StartedAt,
    DateTime? ExpiresAt,
    DateTime? CanceledAt
);


