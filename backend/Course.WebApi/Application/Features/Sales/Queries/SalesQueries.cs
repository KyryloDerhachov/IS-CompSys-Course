namespace Course.WebApi.Features.Sales.Queries;
using Course.WebApi.Features.Sales.DTOs;
using MediatR;

public record GetReceiptByIdQuery(Guid Id) : IRequest<ReceiptDto>;
public record GetReceiptsHistoryQuery() : IRequest<List<ReceiptDto>>;
public record GetReturnsHistoryQuery : IRequest<IEnumerable<ReturnTransactionDto>>;