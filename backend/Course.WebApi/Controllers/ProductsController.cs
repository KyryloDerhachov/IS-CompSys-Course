using System.Security.Claims;
using Course.WebApi.Features.Products.Commands;
using Course.WebApi.Features.Products.DTOs;
using Course.WebApi.Features.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Course.WebApi.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IMediator mediator, ILogger<ProductsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ProductDto>))]
    public async Task<ActionResult<List<ProductDto>>> GetAll([FromQuery] bool includeInactive = false)
    {
        var query = new GetProductsQuery(includeInactive);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetById([FromRoute] Guid id)
    {
        try
        {
            var query = new GetProductByIdQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Product not found with id {Id}", id);
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateProductCommand command)
    {
        try
        {
            var commandWithUser = command with { CurrentUserId = GetCurrentUserId() };
           
            var productId = await _mediator.Send(commandWithUser);
           
            return CreatedAtAction(nameof(GetById), new { id = productId }, productId);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Invalid operation when creating product");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateProductDTO input)
    {
        try
        {
            var command = new UpdateProductCommand(
                Id: id,
                Name: input.Name,
                ClassId: input.ClassId,
                CategoryId: input.CategoryId,
                Unit: input.Unit,
                Version: input.Version,
                Attributes: input.Attributes,
                CurrentUserId: GetCurrentUserId()
            );
            await _mediator.Send(command);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Product not found when updating. Id: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Conflict when updating product {Id}", id);
            return StatusCode(StatusCodes.Status409Conflict, new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Deactivate([FromRoute] Guid id, [FromQuery] int version)
    {
        try
        {
            var command = new DeactivateProductCommand(id, version, GetCurrentUserId());
            await _mediator.Send(command);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Product not found when deactivating. Id: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Conflict when deactivating product {Id}", id);
            return StatusCode(StatusCodes.Status409Conflict, new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Activate([FromRoute] Guid id, [FromQuery] int version)
    {
        try
        {
            var command = new ActivateProductCommand(id, version, GetCurrentUserId());
            await _mediator.Send(command);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Product not found when activating. Id: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Conflict when activating product {Id}", id);
            return StatusCode(StatusCodes.Status409Conflict, new { message = ex.Message });
        }
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst("sub")?.Value;
        if (Guid.TryParse(userIdClaim, out var parsedGuid))
        {
            return parsedGuid;
        }
        return null;
    }
}