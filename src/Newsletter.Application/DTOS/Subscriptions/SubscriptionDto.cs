namespace Newsletter.Application.DTOS.Subscriptions;

public record SubscriptionDto(
    Guid Id,
    Guid UserId,
    string ExternalSubscriptionId,
    string Provider,
    string Status,
    DateTime? StartedAt,
    DateTime? ExpiresAt,
    DateTime? CanceledAt,
    DateTime UpdatedAt);