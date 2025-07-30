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

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var allSubscriptions = await _subscriptionService.GetAllAsync();
        return Ok(allSubscriptions);
    }

    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetByUser(Guid userId)
    {
        var subs = await _subscriptionService.GetByUserIdAsync(userId);
        if (subs == null)
            return NotFound();

        return Ok(subs);
    }
    

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSubscriptionRequest request)
    {
        var createdSubscriptionDto = await _subscriptionService.CreateAsync(request);
        return CreatedAtAction(nameof(GetByUser), new { userId = createdSubscriptionDto.UserId }, createdSubscriptionDto);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update([FromBody] UpdateSubscriptionRequest request, Guid id)
    {
        var updatedSubscriptionDto = await _subscriptionService.UpdateAsync(request, id);
        if (updatedSubscriptionDto == null)
            return NotFound();

        return Ok(updatedSubscriptionDto);
    }

    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> DeleteByUser(Guid userId)
    {
        var deleted = await _subscriptionService.DeleteByUserAsync(userId);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
