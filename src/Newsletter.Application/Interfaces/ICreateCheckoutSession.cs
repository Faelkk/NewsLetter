using Newsletter.Application.DTOS.Payments;
using Newsletter.Application.Events;

namespace Newsletter.Application.Interfaces;

public interface ICreateCheckoutSession
{
    Task<string> ExecuteAsync(CreateCheckoutSessionRequest request);
}