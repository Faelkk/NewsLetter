namespace Newsletter.Application.DTOS.Subscriptions;

public record CreateSubscriptionRequest(
    Guid UserId,
    string ExternalSubscriptionId,
    string Provider);