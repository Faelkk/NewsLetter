using Newsletter.Application.Events;

namespace Newsletter.Application.Interfaces;

public interface IConfirmSubscriptionStatus
{
    Task<bool> ExecuteAsync(ConfirmSubscriptionRequest request);
}