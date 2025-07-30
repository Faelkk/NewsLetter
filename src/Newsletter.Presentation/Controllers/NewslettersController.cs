using Microsoft.AspNetCore.Mvc;
using Newsletter.Application.Interfaces;
using Newsletter.Presentation.DTOS;

namespace Newsletter.Presentation.Controllers;

[ApiController]
[Route("[controller]")]
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
        try
        {
            var newsletter = await _newsletterService.GetByIdAsync(userId, newsletterId);
            return Ok(newsletter);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromBody] GenerateNewsletterRequest request)
    {
        if (request.UserId == Guid.Empty)
            return BadRequest(new { message = "UserId não pode ser Guid vazio." });
        
        var newsletter = await _newsletterService.GenerateAndSendAsync(request);
        return Ok(newsletter);
    }

    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> DeleteByUser(Guid userId)
    {
        var success = await _newsletterService.DeleteAsync(userId);
        if (!success)
            return NotFound(new { message = "Nenhuma newsletter encontrada para esse usuário." });

        return NoContent();
    }
    
    [HttpDelete("{userId:guid}/newsletter/{newsletterId:guid}")]
    public async Task<IActionResult> DeleteByUserAndNewLetterId(Guid userId, Guid newsletterId)
    {
        var success = await _newsletterService.DeleteAsyncNewLetterId(userId,newsletterId);
        if (!success)
            return NotFound(new { message = "newsletter não encontrada para esse usuário." });

        return NoContent();
    }
}
