using Newsletter.Application.DTOS.Subscriptions;
using Newsletter.Application.Interfaces;
using Newsletter.Domain.Entities;
using Newsletter.Domain.Interfaces;
using Newsletter.Presentation.DTOS;

namespace Newsletter.Application.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly ISubscriptionRepository _subscriptionRepository;

    public SubscriptionService(ISubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<IEnumerable<SubscriptionDto>> GetAllAsync()
    {
        var subscriptions = await _subscriptionRepository.GetAllAsync();
        return subscriptions.Select(MapToDto);
    }

    
    public async Task<SubscriptionDto?> GetByUserIdAsync(Guid userId)
    {
        var subscription = await _subscriptionRepository.GetByUserIdAsync(userId);
        if (subscription == null)
            return null;

        return MapToDto(subscription);
    }

    public async Task<SubscriptionDto> CreateAsync(CreateSubscriptionRequest request)
    {
        var entity = new Subscription
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            ExternalSubscriptionId = request.ExternalSubscriptionId,
            Provider = request.Provider ?? "MercadoPago",
            Status = "active",
            StartedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMonths(1),
            CanceledAt = null,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _subscriptionRepository.CreateAsync(entity);
        return MapToDto(created);
    }

    public async Task<SubscriptionDto?> UpdateAsync(UpdateSubscriptionRequest request, Guid id)
    {
        var existing = await _subscriptionRepository.GetByUserIdAsync(request.UserId);
        if (existing == null)
            return null;

        existing.ExternalSubscriptionId = request.ExternalSubscriptionId ?? existing.ExternalSubscriptionId;
        existing.Provider = request.Provider ?? existing.Provider;
        existing.Status = request.Status ?? existing.Status;
        existing.StartedAt = request.StartedAt ?? existing.StartedAt;
        existing.ExpiresAt = request.ExpiresAt ?? existing.ExpiresAt;
        existing.CanceledAt = request.CanceledAt ?? existing.CanceledAt;
        existing.UpdatedAt = DateTime.UtcNow;

        var updated = await _subscriptionRepository.UpdateAsync(existing);
        if (updated == null)
            return null;

        return MapToDto(updated);
    }

    public async Task<bool> DeleteByUserAsync(Guid userId)
    {
        return await _subscriptionRepository.DeleteAsync(userId);
    }

    private SubscriptionDto MapToDto(Subscription sub)
    {
        return new SubscriptionDto(
            Id: sub.Id,
            UserId: sub.UserId,
            ExternalSubscriptionId: sub.ExternalSubscriptionId,
            Provider: sub.Provider,
            Status: sub.Status,
            StartedAt: sub.StartedAt,
            ExpiresAt: sub.ExpiresAt,
            CanceledAt: sub.CanceledAt,
            UpdatedAt: sub.UpdatedAt
        );
    }
}
