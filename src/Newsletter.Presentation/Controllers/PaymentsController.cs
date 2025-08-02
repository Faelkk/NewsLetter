using Microsoft.AspNetCore.Mvc;
using Newsletter.Application.DTOS.Payments;
using Newsletter.Application.UseCases;
using Newsletter.Infrastructure.Stripe;
using Stripe;

namespace Newsletter.Presentation.Controllers;

[ApiController]
[Route("payments")]
public class PaymentsController : ControllerBase
{
    private readonly CreateCheckoutSession _createCheckoutSession;
    private readonly StripeWebhookHandler _webhookHandler;
    private readonly ILogger<PaymentsController> _logger;
    private readonly IConfiguration _configuration;

    public PaymentsController(
        CreateCheckoutSession createCheckoutSession,
        StripeWebhookHandler webhookHandler,
        ILogger<PaymentsController> logger,
        IConfiguration configuration)
    {
        _createCheckoutSession = createCheckoutSession;
        _webhookHandler = webhookHandler;
        _logger = logger;
        _configuration = configuration;
    }


    [HttpPost("checkout")]
    public async Task<IActionResult> CreateCheckoutSession([FromBody] CreateCheckoutSessionRequest request)
    {
        var sessionUrl = await _createCheckoutSession.ExecuteAsync(request);
        return Ok(new { Url = sessionUrl });
    }


    [HttpPost("webhook")]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var signature = Request.Headers["Stripe-Signature"];

        try
        {
            var secret = _configuration["Stripe:WebhookSecret"];
            var stripeEvent = EventUtility.ConstructEvent(json, signature, secret);

            await _webhookHandler.HandleAsync(stripeEvent);
            return Ok();
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Erro ao processar webhook Stripe");
            return BadRequest();
        }
    }
}
