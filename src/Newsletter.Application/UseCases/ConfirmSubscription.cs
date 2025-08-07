using Newsletter.Application.Events;
using Newsletter.Application.Interfaces;
using Newsletter.Domain.Interfaces;

namespace Newsletter.Application.UseCases;

public class ConfirmSubscriptionStatus : IConfirmSubscriptionStatus 
{
    private readonly ISubscriptionRepository _subscriptionRepository;

    public ConfirmSubscriptionStatus(ISubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<bool> ExecuteAsync(ConfirmSubscriptionRequest request)
    {
        var subscription = await _subscriptionRepository.GetBySubscriptionIdAsync(request.SubscriptionId);
        if (subscription == null) return false;

        subscription.Status = request.Status;
    
        subscription.UpdatedAt = DateTime.UtcNow;
        subscription.StartedAt = DateTime.UtcNow;
        subscription.NextDeliveryDate = DateTime.UtcNow.Date;

        subscription.Plan = request.PlanId;
        subscription.ExternalSubscriptionId = request.ExternalSubscriptionId;
        
        subscription.ExpiresAt = CalculateExpiry(subscription.StartedAt ?? DateTime.UtcNow, request.PlanId).Date;
        

        await _subscriptionRepository.UpdateAsync(subscription);
        return true;
    }

    private DateTime CalculateExpiry(DateTime startDate, string planId)
    {
        return planId switch
        {
            "price_1RqpK7DA64uC2BNAwFZ0nWAt" => startDate.AddMonths(1),     
            "price_1RqpK7DA64uC2BNAPTIONdHu" => startDate.AddMonths(3),    
            "price_1RqpK7DA64uC2BNAOTP3Bt2d" => startDate.AddYears(1),    
            _ => startDate.AddMonths(1) 
        };
    }

}



