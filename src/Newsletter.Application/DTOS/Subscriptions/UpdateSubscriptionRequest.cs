using Newsletter.Domain.Enums;

namespace Newsletter.Presentation.DTOS;

public record UpdateSubscriptionRequest(
    string? ExternalSubscriptionId,
    string? Provider,
    string? Status,
    SubscriptionPlan? Plan,
    DateTime? StartedAt,
    DateTime? ExpiresAt,
    DateTime? CanceledAt
);


