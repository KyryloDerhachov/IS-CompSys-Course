using Course.WebApi.Features.Users.Commands;
using Course.WebApi.Features.Users.DTOs;
using Course.WebApi.Features.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Course.WebApi.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IMediator mediator, ILogger<UsersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginCommand command)
        => await _mediator.Send(command);

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateUserCommand command)
    {
        try
        {
            return await _mediator.Send(command);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Invalid operation when creating user");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAll([FromQuery] bool includeInactive = false)
        => await _mediator.Send(new GetUsersQuery(includeInactive));

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetById(Guid id)
    {
        try
        {
            return await _mediator.Send(new GetUserByIdQuery(id));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "User not found with id {Id}", id);
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut("{id}/deactivate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        try
        {
            await _mediator.Send(new DeactivateUserCommand(id));
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "User not found when deactivating. Id: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Conflict when deactivating user {Id}", id);
            return StatusCode(StatusCodes.Status409Conflict, new { message = ex.Message });
        }
    }

    [HttpPut("{id}/roles")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateRoles(Guid id, [FromBody] List<Guid> roleIds)
    {
        try
        {
            var command = new UpdateUserRolesCommand(id, roleIds);
            await _mediator.Send(command);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "User not found when updating roles. Id: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Conflict when updating roles for user {Id}", id);
            return StatusCode(StatusCodes.Status409Conflict, new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserCommand command)
    {
        try
        {
            await _mediator.Send(command with { Id = id });
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "User not found. Id: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Conflict when updating user {Id}", id);
            return StatusCode(StatusCodes.Status409Conflict, new { message = ex.Message });
        }
    }
}