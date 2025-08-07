namespace Newsletter.Presentation.DTOS;

public record SubscriptionWithUserEmail(

    Guid SubscriptionId,
    Guid UserId,
    string UserEmail,
    List<string> Interests,
    DateTime NextDeliveryDate
);
