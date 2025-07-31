using Newsletter.Domain.Enums;

namespace Newsletter.Application.DTOS.Subscriptions;

public record SubscriptionDto(
    Guid Id,
    Guid UserId,
    string ExternalSubscriptionId,
    string Provider,
    string Status,
    SubscriptionPlan Plan,
    DateTime? StartedAt,
    DateTime? ExpiresAt,
    DateTime? CanceledAt,
    DateTime UpdatedAt);