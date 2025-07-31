using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newsletter.Application.DTOS.Subscriptions;
using Newsletter.Application.Interfaces;
using Newsletter.Application.Services;
using Newsletter.Presentation.DTOS;

namespace Newsletter.Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionsController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var allSubscriptions = await _subscriptionService.GetAllAsync();
        return Ok(allSubscriptions);
    }

    [Authorize]
    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetByUser(Guid userId)
    {
        var subs = await _subscriptionService.GetByUserIdAsync(userId);
        if (subs == null)
            return NotFound("Subscription not found");

        return Ok(subs);
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSubscriptionRequest request)
    {
        try
        {
            var subscription = await _subscriptionService.CreateAsync(request);
            return CreatedAtAction(nameof(GetByUser), new { userId = subscription.UserId }, subscription);

        }
        catch (Exception ex) when (ex.Message.Contains("Usuário não encontrado"))
        {
            return NotFound(new { message = ex.Message });
        }
    }
    
    [Authorize]
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update([FromBody] UpdateSubscriptionRequest request, Guid id)
    {
        if (
            request.ExternalSubscriptionId is null &&
            request.Provider is null &&
            request.Status is null &&
            request.StartedAt is null &&
            request.ExpiresAt is null &&
            request.CanceledAt is null)
        {
            return BadRequest("É necessário informar ao menos um campo para atualização.");
        }

        var updatedSubscriptionDto = await _subscriptionService.UpdateAsync(request, id);
        if (updatedSubscriptionDto == null)
            return NotFound("Subscription not found");

        return Ok(updatedSubscriptionDto);
    }


    [Authorize]
    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> DeleteByUser(Guid userId)
    {
        var deleted = await _subscriptionService.DeleteByUserAsync(userId);
        if (!deleted)
            return NotFound("Subscription not found");

        return NoContent();
    }
}

