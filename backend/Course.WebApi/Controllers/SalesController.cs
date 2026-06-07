using System.Security.Claims;
using Course.WebApi.Features.Sales.Commands;
using Course.WebApi.Features.Sales.DTOs;
using Course.WebApi.Features.Sales.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Course.WebApi.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SalesController> _logger;

    public SalesController(IMediator mediator, ILogger<SalesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("receipts")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CreateReceipt([FromBody] CreateReceiptInputModel input)
    {
        try
        {
            var items = input.Items.Select(i => new SaleItemInput(i.ProductId, i.quantity, i.price)).ToList();
            var command = new CreateReceiptCommand(items, GetUserId());
           
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetReceiptById), new { id }, id);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Invalid operation when creating receipt");
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("receipts/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReceiptDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReceiptDto>> GetReceiptById(Guid id)
    {
        try { return Ok(await _mediator.Send(new GetReceiptByIdQuery(id))); }
        catch (KeyNotFoundException ex) 
        { 
            _logger.LogWarning(ex, "Receipt not found with id {Id}", id);
            return NotFound(new { message = ex.Message }); 
        }
    }

    [HttpGet("receipts")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ReceiptDto>))]
    public async Task<ActionResult<List<ReceiptDto>>> GetHistory()
        => Ok(await _mediator.Send(new GetReceiptsHistoryQuery()));

    [HttpPost("receipts/{id:guid}/returns")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> ProcessReturn(Guid id, [FromBody] CreateReturnInputModel input)
    {
        try
        {
            var items = input.Items.Select(i => new ReturnItemInput(i.ProductId, i.Quantity)).ToList();
            var command = new ProcessReturnCommand(id, items, GetUserId());
           
            var returnTxId = await _mediator.Send(command);
            return Ok(returnTxId);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Invalid operation when processing return for receipt {Id}", id);
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Receipt not found when processing return. Id: {Id}", id);
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("returns")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ReturnTransactionDto>))]
    public async Task<ActionResult<List<ReturnTransactionDto>>> GetReturnsHistory()
        => Ok(await _mediator.Send(new GetReturnsHistoryQuery()));

    private Guid? GetUserId() => Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : null;
}

public record ReturnItemInputModel(Guid ProductId, decimal Quantity);
public record CreateReturnInputModel(List<ReturnItemInputModel> Items);
public record SaleItemInputModel(Guid ProductId, decimal quantity, decimal price);
public record CreateReceiptInputModel(List<SaleItemInputModel> Items);