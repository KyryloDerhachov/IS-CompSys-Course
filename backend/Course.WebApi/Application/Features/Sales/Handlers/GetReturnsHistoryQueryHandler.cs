using Course.WebApi.Features.Sales.DTOs;
using Course.WebApi.Features.Sales.Queries;
using Course.WebApi.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Course.WebApi.Features.Sales.Handlers;

public class GetReturnsHistoryQueryHandler : IRequestHandler<GetReturnsHistoryQuery, IEnumerable<ReturnTransactionDto>>
{
    private readonly ApplicationDbContext _context;

    public GetReturnsHistoryQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ReturnTransactionDto>> Handle(GetReturnsHistoryQuery request, CancellationToken cancellationToken)
    {
        var returns = await _context.ReturnTransactions
            .Include(r => r.Items)
            .AsNoTracking()
            .OrderByDescending(r => r.CreatedAt) 
            .ToListAsync(cancellationToken);

        return returns.Select(r => new ReturnTransactionDto(
            r.Id,
            r.ReceiptId,
            r.CreatedAt,
            r.Items.Sum(i => i.Quantity * i.Price),
            r.Items.Select(i => new ReturnItemDto(i.ProductId, i.BatchId, i.Quantity, i.Price, i.ReturnedToStock)).ToList()
        ));
    }
}