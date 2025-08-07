using Newsletter.Application.DTOS.Subscriptions;
using Newsletter.Application.Interfaces;
using Newsletter.Domain.Entities;
using Newsletter.Domain.Interfaces;
using Newsletter.Infrastructure.Interfaces;
using Newsletter.Presentation.DTOS;

namespace Newsletter.Application.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IStripeSubscriptionService _stripeSubscriptionService;

    public SubscriptionService(ISubscriptionRepository subscriptionRepository,IUserRepository userRepository)
    {
        _subscriptionRepository = subscriptionRepository;
        _userRepository = userRepository;
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
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user is null)
            throw new Exception("Usuário não encontrado.");

        var existingSubscription = await _subscriptionRepository.GetByUserIdAsync(request.UserId);
        if (existingSubscription != null)
            throw new Exception("Usuário já possui uma assinatura.");

        var entity = new Subscription
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            ExternalSubscriptionId = null,
            Plan = null,
            Provider = request.Provider ?? "Stripe",
            NextDeliveryDate = null,
            Status = "pending",
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
        var existing = await _subscriptionRepository.GetByUserIdAsync(id);
        if (existing == null)
            return null;

        existing.ExternalSubscriptionId = request.ExternalSubscriptionId ?? existing.ExternalSubscriptionId;
        existing.Plan = request.Plan ?? existing.Plan; 
        existing.Provider = request.Provider ?? existing.Provider;
        existing.Status = request.Status ?? existing.Status;
        existing.NextDeliveryDate = request.NextDeliveryDate ?? existing.NextDeliveryDate;
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
        var subscription = await _subscriptionRepository.GetByUserIdAsync(userId);
        if (subscription == null)
            return false;

        if (!string.IsNullOrWhiteSpace(subscription.ExternalSubscriptionId) &&
            subscription.Provider == "Stripe")
        {
            try
            {
                await _stripeSubscriptionService.CancelSubscriptionAsync(subscription.ExternalSubscriptionId!);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao cancelar assinatura no Stripe: {ex.Message}");
            }
        }

        return await _subscriptionRepository.DeleteAsync(userId);
    }

    private SubscriptionDto MapToDto(Subscription sub)
    {
        return new SubscriptionDto(
            Id: sub.Id,
            UserId: sub.UserId,
            ExternalSubscriptionId: sub.ExternalSubscriptionId,
            Plan: sub.Plan,
            Provider: sub.Provider,
            Status: sub.Status,
            NextDeliveryDate: sub.NextDeliveryDate,
            StartedAt: sub.StartedAt,
            ExpiresAt: sub.ExpiresAt,
            CanceledAt: sub.CanceledAt,
            UpdatedAt: sub.UpdatedAt
        );
    }
}



