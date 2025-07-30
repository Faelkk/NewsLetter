using Newsletter.Application.DTOS.Subscriptions;
using Newsletter.Application.Interfaces;
using Newsletter.Presentation.DTOS;

namespace Newsletter.Application.Services;

public class SubscriptionService : ISubscriptionService
{
    public Task<SubscriptionDto> GetByUserIdAsync(Guid userId)
    {
        var subscription = new SubscriptionDto(
            Id: Guid.NewGuid(),
            UserId: userId,
            ExternalSubscriptionId: "sub_123456",
            Provider: "MercadoPago",
            Status: "active",
            StartedAt: DateTime.UtcNow.AddDays(-10),
            ExpiresAt: DateTime.UtcNow.AddDays(20),
            CanceledAt: null,
            UpdatedAt: DateTime.UtcNow
        );

        return Task.FromResult(subscription);
    }

    public Task<SubscriptionDto> GetBySubscriptionIdAsync(Guid userId, Guid subscriptionId)
    {
        var subscription = new SubscriptionDto(
            Id: subscriptionId,
            UserId: userId,
            ExternalSubscriptionId: "sub_654321",
            Provider: "MercadoPago",
            Status: "canceled",
            StartedAt: DateTime.UtcNow.AddDays(-30),
            ExpiresAt: DateTime.UtcNow.AddDays(-5),
            CanceledAt: DateTime.UtcNow.AddDays(-6),
            UpdatedAt: DateTime.UtcNow
        );

        return Task.FromResult(subscription);
    }

    public Task<SubscriptionDto> CreateAsync(CreateSubscriptionRequest request)
    {
        var newSubscription = new SubscriptionDto(
            Id: Guid.NewGuid(),
            UserId: request.UserId,
            ExternalSubscriptionId: request.ExternalSubscriptionId,
            Provider: request.Provider,
            Status: "active",
            StartedAt: DateTime.UtcNow,
            ExpiresAt: DateTime.UtcNow.AddMonths(1),
            CanceledAt: null,
            UpdatedAt: DateTime.UtcNow
        );

        return Task.FromResult(newSubscription);
    }

    public Task<SubscriptionDto> UpdateAsync(UpdateSubscriptionRequest request,Guid id)
    {
        var updated = new SubscriptionDto(
            Id: id,
            UserId: request.UserId,
            ExternalSubscriptionId: request.ExternalSubscriptionId ?? "sub_default",
            Provider: request.Provider ?? "MercadoPago",
            Status: request.Status ?? "active",
            StartedAt: request.StartedAt,
            ExpiresAt: request.ExpiresAt,
            CanceledAt: request.CanceledAt,
            UpdatedAt: DateTime.UtcNow
        );

        return Task.FromResult(updated);
    }

    public Task<bool> DeleteByUserAsync(Guid userId)
    {
        return Task.FromResult(true);
    }
}


