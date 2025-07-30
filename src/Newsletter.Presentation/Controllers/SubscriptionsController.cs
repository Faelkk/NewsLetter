using Microsoft.AspNetCore.Mvc;
using Newsletter.Application.DTOS.Subscriptions;
using Newsletter.Application.Interfaces;
using Newsletter.Application.Services;
using Newsletter.Presentation.DTOS;

namespace Newsletter.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionsController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }


    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetByUser(Guid userId)
    {
        var subs = await _subscriptionService.GetByUserIdAsync(userId);
        
 
        
        return Ok(subs);
    }
    
    [HttpGet("{userId:guid}/subscription/{subscriptionId:guid}")]
    public async Task<IActionResult> GetBySubscriptionId(Guid userId, Guid subscriptionId)
    {
        var subscription = await _subscriptionService.GetBySubscriptionIdAsync(userId, subscriptionId);
        

        return Ok(subscription);
    }

    

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSubscriptionRequest request)
    {
        var createdSubscriptionDto = await _subscriptionService.CreateAsync(request);
        return Ok(createdSubscriptionDto);
    }

    [HttpPatch("{userId:guid}")]
    public async Task<IActionResult> Update([FromBody] UpdateSubscriptionRequest request,Guid id)
    {
        var updatedSubscriptionDto = await _subscriptionService.UpdateAsync(request,id);
        
        return Ok(updatedSubscriptionDto);
    }
    
    [HttpDelete("{userId:guid}")] 
    public async Task<IActionResult> DeleteByUser(Guid userId) {
        await _subscriptionService.DeleteByUserAsync(userId);
        return NoContent();
        }
    
}