using Newsletter.Application.DTOS.Payments;
using Newsletter.Application.UseCases;

namespace Newsletter.Application.Interfaces;

public interface IStripeCheckoutService
{
    Task<string> CreateCheckoutSessionAsync(CreateCheckoutSessionRequest request);
}