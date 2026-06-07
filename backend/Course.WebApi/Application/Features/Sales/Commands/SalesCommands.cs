namespace Course.WebApi.Features.Sales.Commands;

using Course.WebApi.Features.Sales.DTOs;
using MediatR;



public record CreateReceiptCommand(
    List<SaleItemInput> Items, 
    Guid? CurrentUserId) : IRequest<Guid>;


public record ProcessReturnCommand(
    Guid ReceiptId, 
    List<ReturnItemInput> Items, 
    Guid? CurrentUserId) : IRequest<Guid>;