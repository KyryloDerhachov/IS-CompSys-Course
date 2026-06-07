using System.Security.Claims;
using Course.WebApi.Features.Supplies.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Course.WebApi.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SuppliesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SuppliesController> _logger;

    public SuppliesController(IMediator mediator, ILogger<SuppliesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateSupplyInputModel input)
    {
        try
        {
            var items = input.Items.Select(i => new CreateSupplyItemInput(i.ProductId, i.Quantity, i.PurchasePrice, i.ShelfLifeDays)).ToList();
            var command = new CreateSupplyCommand(input.SupplierName, input.SupplyDate, items, GetUserId());
           
            var id = await _mediator.Send(command);
            return StatusCode(StatusCodes.Status201Created, id);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Product not found when creating supply");
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Invalid operation when creating supply");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}/post")]
    public async Task<IActionResult> PostSupply(Guid id, [FromQuery] int version)
    {
        try
        {
            await _mediator.Send(new PostSupplyCommand(id, version, GetUserId()));
            return NoContent();
        }
        catch (KeyNotFoundException ex) 
        { 
            _logger.LogWarning(ex, "Supply not found with id {Id}", id);
            return NotFound(new { message = ex.Message }); 
        }
        catch (InvalidOperationException ex) 
        { 
            _logger.LogError(ex, "Conflict when posting supply {Id}", id);
            return StatusCode(StatusCodes.Status409Conflict, new { message = ex.Message }); 
        }
    }

    private Guid? GetUserId() => Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : null;
}

public record CreateSupplyItemInputModel(Guid ProductId, decimal Quantity, decimal PurchasePrice, int ShelfLifeDays);
public record CreateSupplyInputModel(string SupplierName, DateTime SupplyDate, List<CreateSupplyItemInputModel> Items);