using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Course.WebApi.Application.Feedback.Commands.CreateFeedback;
using Course.WebApi.Application.Feedback.Commands.RespondToFeedback;
using Course.WebApi.Application.Feedback.Queries.GetAllFeedbacks;
using Course.WebApi.Features.Feedback.DTOs;

namespace Course.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeedbackController : ControllerBase
{
    private readonly ISender _mediator;

    public FeedbackController(ISender mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateFeedbackCommand command)
    {
        try
        {
            var feedbackId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetAll), new { id = feedbackId }, feedbackId);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/respond")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Respond(Guid id, [FromBody] string response)
    {
        try
        {
            var managerIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(managerIdClaim) || !Guid.TryParse(managerIdClaim, out var managerId))
            {
                return Unauthorized(new { message = "Manager ID could not be determined from the token." });
            }

            var command = new RespondToFeedbackCommand(id, response, managerId);
            await _mediator.Send(command);
            
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<FeedbackDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FeedbackDto>>> GetAll()
    {
        var query = new GetAllFeedbacksQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}