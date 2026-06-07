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
[Route("api/product-categories")]
public class ProductCategoriesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductCategoriesController> _logger;

    public ProductCategoriesController(IMediator mediator, ILogger<ProductCategoriesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductCategoryDto>>> GetAll([FromQuery] bool includeInactive = false)
        => Ok(await _mediator.Send(new GetProductCategoriesQuery(includeInactive)));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductCategoryDto>> GetById(Guid id)
    {
        try { return Ok(await _mediator.Send(new GetProductCategoryByIdQuery(id))); }
        catch (KeyNotFoundException ex) 
        { 
            _logger.LogWarning(ex, "Product category not found with id {Id}", id);
            return NotFound(new { message = ex.Message }); 
        }
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateCategoryInput input)
    {
        var command = new CreateProductCategoryCommand(input.ParentId, input.Code, input.Name, input.Description, GetUserId());
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}/description")]
    public async Task<IActionResult> UpdateDescription(Guid id, [FromBody] UpdateDescriptionInput input)
    {
        try
        {
            await _mediator.Send(new UpdateProductCategoryDescriptionCommand(id, input.Description, input.Version, GetUserId()));
            return NoContent();
        }
        catch (KeyNotFoundException ex) 
        { 
            _logger.LogWarning(ex, "Product category not found when updating description. Id: {Id}", id);
            return NotFound(new { message = ex.Message }); 
        }
        catch (InvalidOperationException ex) 
        { 
            _logger.LogError(ex, "Conflict when updating description for category {Id}", id);
            return StatusCode(StatusCodes.Status409Conflict, new { message = ex.Message }); 
        }
    }

    [HttpPut("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id, [FromQuery] int version)
    {
        try
        {
            await _mediator.Send(new DeactivateProductCategoryCommand(id, version, GetUserId()));
            return NoContent();
        }
        catch (KeyNotFoundException ex) 
        { 
            _logger.LogWarning(ex, "Product category not found when deactivating. Id: {Id}", id);
            return NotFound(new { message = ex.Message }); 
        }
        catch (InvalidOperationException ex) 
        { 
            _logger.LogError(ex, "Conflict when deactivating category {Id}", id);
            return StatusCode(StatusCodes.Status409Conflict, new { message = ex.Message }); 
        }
    }

    [HttpPut("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id, [FromQuery] int version)
    {
        try
        {
            await _mediator.Send(new ActivateProductCategoryCommand(id, version, GetUserId()));
            return NoContent();
        }
        catch (KeyNotFoundException ex) 
        { 
            _logger.LogWarning(ex, "Product category not found when activating. Id: {Id}", id);
            return NotFound(new { message = ex.Message }); 
        }
        catch (InvalidOperationException ex) 
        { 
            _logger.LogError(ex, "Conflict when activating category {Id}", id);
            return StatusCode(StatusCodes.Status409Conflict, new { message = ex.Message }); 
        }
    }

    private Guid? GetUserId() => Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : null;
}

public record CreateCategoryInput(Guid? ParentId, string Code, string Name, string? Description);