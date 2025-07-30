using Microsoft.AspNetCore.Mvc;
using Newsletter.Application.Interfaces;
using Newsletter.Application.Services;
using Newsletter.Presentation.DTOS;

namespace Newsletter.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NewslettersController : ControllerBase
{
    private readonly INewsletterService _newsletterService;

    public NewslettersController(INewsletterService newsletterService)
    {
        _newsletterService = newsletterService;
    }

 
    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetByUser(Guid userId)
    {
        var newsletters = await _newsletterService.GetByUserIdAsync(userId);
        return Ok(newsletters);
    }
    
    [HttpGet("{userId:guid}/newsletter/{newsletterId:guid}")]
    public async Task<IActionResult> GetById(Guid userId, Guid newsletterId)
    {
        var newsletter = await _newsletterService.GetByIdAsync(userId, newsletterId);

        if (newsletter is null)
            return NotFound();

        return Ok(newsletter);
    }


    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromBody] GenerateNewsletterRequest request)
    {
        var newsletter = await _newsletterService.GenerateAndSendAsync(request);
        return Ok(newsletter);
    }


    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> DeleteByUser(Guid userId)
    {
        await _newsletterService.DeleteAsync(userId);
        return NoContent();
    }
}