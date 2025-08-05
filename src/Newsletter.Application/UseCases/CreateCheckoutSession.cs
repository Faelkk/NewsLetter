using Newsletter.Application.DTOS.Payments;
using Newsletter.Application.Interfaces;

namespace Newsletter.Application.UseCases;

public class CreateCheckoutSession: ICreateCheckoutSession
{
    private readonly IStripeCheckoutService _stripeService;
    
    
    public CreateCheckoutSession(IStripeCheckoutService stripeService)
    {
        _stripeService = stripeService;
    }

    public async Task<string> ExecuteAsync(CreateCheckoutSessionRequest request)
    {
        return await _stripeService.CreateCheckoutSessionAsync(request);
    }
}