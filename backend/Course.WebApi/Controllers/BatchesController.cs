using System.Security.Claims;
using Course.WebApi.Features.Batches.Commands;
using Course.WebApi.Features.Batches.DTOs;
using Course.WebApi.Features.Batches.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Course.WebApi.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BatchesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<BatchesController> _logger;

    public BatchesController(IMediator mediator, ILogger<BatchesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<BatchDto>))]
    public async Task<ActionResult<List<BatchDto>>> GetBatches([FromQuery] Guid? productId)
        => Ok(await _mediator.Send(new GetBatchesQuery(productId)));

    [HttpGet("expiring-stocks")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<BatchDto>))]
    public async Task<ActionResult<List<BatchDto>>> GetExpiringStocks([FromQuery] int daysThreshold = 7)
        => Ok(await _mediator.Send(new GetExpiringBatchesQuery(daysThreshold)));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BatchDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BatchDto>> GetById(Guid id)
    {
        try { return Ok(await _mediator.Send(new GetBatchByIdQuery(id))); }
        catch (KeyNotFoundException ex) 
        { 
            _logger.LogWarning(ex, "Batch not found with id {Id}", id);
            return NotFound(new { message = ex.Message }); 
        }
    }

    [HttpPut("{id:guid}/reduce-stock")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ReduceStock(Guid id, [FromBody] ReduceStockInputModel input)
    {
        try
        {
            var command = new ReduceBatchStockCommand(id, input.Quantity, input.Version, GetUserId());
            await _mediator.Send(command);
            return NoContent();
        }
        catch (KeyNotFoundException ex) 
        { 
            _logger.LogWarning(ex, "Batch not found when reducing stock. Id: {Id}", id);
            return NotFound(new { message = ex.Message }); 
        }
        catch (InvalidOperationException ex) 
        { 
            _logger.LogError(ex, "Conflict when reducing stock for batch {Id}", id);
            return StatusCode(StatusCodes.Status409Conflict, new { message = ex.Message }); 
        }
    }

    private Guid? GetUserId() => Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : null;
}

public class ReduceStockInputModel
{
    public decimal Quantity { get; set; }
    public int Version { get; set; }
}