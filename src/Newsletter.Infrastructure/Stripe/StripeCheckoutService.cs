using Microsoft.Extensions.Options;
using Stripe;
using Newsletter.Application.DTOS.Payments;
using Newsletter.Application.Interfaces;
using Newsletter.Domain.Interfaces;
using Stripe.Checkout;

namespace Newsletter.Infrastructure.Stripe;

public class StripeCheckoutService : IStripeCheckoutService
{
    private readonly StripeSettings _stripeSettings;
    private readonly ISubscriptionRepository _subscriptionRepository;
    public StripeCheckoutService(IOptions<StripeSettings> stripeOptions,ISubscriptionRepository subscriptionRepository)
    {
        _stripeSettings = stripeOptions.Value;
        _subscriptionRepository = subscriptionRepository;
        StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
        
        
    }

    public async Task<string> CreateCheckoutSessionAsync(CreateCheckoutSessionRequest request) 
    {
        var existingSubscription = await _subscriptionRepository.GetBySubscriptionIdAndUserIdAsync(request.SubscriptionId, request.UserId);

        if (existingSubscription == null)
        {
            throw new InvalidOperationException("Subscription does not exist for this user, cannot create checkout session.");
        }
        
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    Price = request.PlanId, 
                    Quantity = 1
                }
            },
            Mode = "subscription",
            SuccessUrl = request.SuccessUrl,
            CancelUrl = request.CancelUrl,
            ClientReferenceId = request.UserId.ToString(),
            SubscriptionData = new SessionSubscriptionDataOptions
            {
                Metadata = new Dictionary<string, string>
                {
                    { "subscription_id", request.SubscriptionId.ToString() }
                }
            }
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options);
        return session.Url;
    }
}


