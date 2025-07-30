using Microsoft.AspNetCore.Mvc;
using Newsletter.Application.DTOS.Users;
using Newsletter.Application.Interfaces;

namespace Newsletter.Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var users = await _userService.GetAsync();
        return Ok(users);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user is null)
            return NotFound("Usuario não encontrado");

        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        try
        {
            var createdUser = await _userService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
        }
        catch (Exception ex) when (ex.Message.Contains("E-mail já cadastrado"))
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request)
    {
        if (request.Name is null &&
            request.Email is null &&
            request.Plan is null &&
            (request.Interests is null || !request.Interests.Any()))
        {
            return BadRequest("É necessário informar ao menos um campo para atualização.");
        }
        
        var updatedUser = await _userService.UpdateAsync(id, request);
        if (updatedUser is null)
            return NotFound("Usuario não encontrado");

        return Ok(updatedUser);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _userService.DeleteAsync(id);
        if (!result)
            return NotFound("Usuario não encontrado");

        return NoContent();
    }
}

