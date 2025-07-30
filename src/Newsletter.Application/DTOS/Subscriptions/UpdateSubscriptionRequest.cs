namespace Newsletter.Presentation.DTOS;

public record UpdateSubscriptionRequest(
    string? ExternalSubscriptionId,
    string? Provider,
    string? Status,
    DateTime? StartedAt,
    DateTime? ExpiresAt,
    DateTime? CanceledAt
);


