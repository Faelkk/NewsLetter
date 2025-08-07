using Microsoft.Extensions.Options;
using Newsletter.Application.Interfaces;
using Newsletter.Infrastructure.Interfaces;
using Newsletter.Infrastructure.Settings;
using Stripe;

namespace Newsletter.Infrastructure.Stripe
{
    public class StripeSubscriptionService : IStripeSubscriptionService
    {
        private readonly StripeSettings _stripeSettings;

        public StripeSubscriptionService(IOptions<StripeSettings> stripeOptions)
        {
            _stripeSettings = stripeOptions.Value;
            
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
        }

        public async Task CancelSubscriptionAsync(string externalSubscriptionId)
        {
            var service = new SubscriptionService();
            await service.CancelAsync(externalSubscriptionId);
        }
    }
}