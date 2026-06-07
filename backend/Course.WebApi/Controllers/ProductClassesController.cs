using System.Security.Claims;
using Course.WebApi.Features.Classification.Commands;
using Course.WebApi.Features.Classification.DTOs;
using Course.WebApi.Features.Classification.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Course.WebApi.Controllers;
[Authorize]
[ApiController]
[Route("api/product-classes")]
public class ProductClassesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductClassesController> _logger;

    public ProductClassesController(IMediator mediator, ILogger<ProductClassesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductClassDto>>> GetAll([FromQuery] bool includeInactive = false)
        => Ok(await _mediator.Send(new GetProductClassesQuery(includeInactive)));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductClassDto>> GetById(Guid id)
    {
        try { return Ok(await _mediator.Send(new GetProductClassByIdQuery(id))); }
        catch (KeyNotFoundException ex) 
        { 
            _logger.LogWarning(ex, "Product class not found with id {Id}", id);
            return NotFound(new { message = ex.Message }); 
        }
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateClassInput input)
    {
        var command = new CreateProductClassCommand(input.Code, input.Name, input.Description, GetUserId());
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}/description")]
    public async Task<IActionResult> UpdateDescription(Guid id, [FromBody] UpdateDescriptionInput input)
    {
        try
        {
            await _mediator.Send(new UpdateProductClassDescriptionCommand(id, input.Description, input.Version, GetUserId()));
            return NoContent();
        }
        catch (KeyNotFoundException ex) 
        { 
            _logger.LogWarning(ex, "Product class not found when updating description. Id: {Id}", id);
            return NotFound(new { message = ex.Message }); 
        }
        catch (InvalidOperationException ex) 
        { 
            _logger.LogError(ex, "Conflict when updating description for class {Id}", id);
            return StatusCode(StatusCodes.Status409Conflict, new { message = ex.Message }); 
        }
    }

    [HttpPut("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id, [FromQuery] int version)
    {
        try
        {
            await _mediator.Send(new DeactivateProductClassCommand(id, version, GetUserId()));
            return NoContent();
        }
        catch (KeyNotFoundException ex) 
        { 
            _logger.LogWarning(ex, "Product class not found when deactivating. Id: {Id}", id);
            return NotFound(new { message = ex.Message }); 
        }
        catch (InvalidOperationException ex) 
        { 
            _logger.LogError(ex, "Conflict when deactivating class {Id}", id);
            return StatusCode(StatusCodes.Status409Conflict, new { message = ex.Message }); 
        }
    }

    [HttpPut("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id, [FromQuery] int version)
    {
        try
        {
            await _mediator.Send(new ActivateProductClassCommand(id, version, GetUserId()));
            return NoContent();
        }
        catch (KeyNotFoundException ex) 
        { 
            _logger.LogWarning(ex, "Product class not found when activating. Id: {Id}", id);
            return NotFound(new { message = ex.Message }); 
        }
        catch (InvalidOperationException ex) 
        { 
            _logger.LogError(ex, "Conflict when activating class {Id}", id);
            return StatusCode(StatusCodes.Status409Conflict, new { message = ex.Message }); 
        }
    }

    private Guid? GetUserId() => Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : null;
}

public record CreateClassInput(string Code, string Name, string? Description);
public record UpdateDescriptionInput(string? Description, int Version);