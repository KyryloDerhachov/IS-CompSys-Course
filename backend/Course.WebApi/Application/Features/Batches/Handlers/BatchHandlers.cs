using Course.WebApi.Application.Interfaces;
using Course.WebApi.Features.Batches.DTOs;
using Course.WebApi.Features.Batches.Queries;
using Course.WebApi.Features.Batches.Commands;
using MediatR;

namespace Course.WebApi.Features.Batches.Handlers;

public class BatchHandlers :
    IRequestHandler<GetBatchesQuery, List<BatchDto>>,
    IRequestHandler<GetExpiringBatchesQuery, List<BatchDto>>,
    IRequestHandler<GetBatchByIdQuery, BatchDto>,
    IRequestHandler<ReduceBatchStockCommand>
{
    private readonly IBatchRepository _batchRepository;
    private readonly IProductRepository _productRepository;

    public BatchHandlers(IBatchRepository batchRepository, IProductRepository productRepository)
    {
        _batchRepository = batchRepository;
        _productRepository = productRepository;
    }

    public async Task<List<BatchDto>> Handle(GetBatchesQuery request, CancellationToken cancellationToken)
    {
        var batches = await _batchRepository.GetAllAsync(); 
        var products = await _productRepository.GetAllAsync(includeInactive: true);

        var query = batches.AsEnumerable();

        if (request.ProductId.HasValue)
        {
            query = query.Where(b => b.ProductId == request.ProductId.Value);
        }

        return query.Select(b => {
            var prod = products.FirstOrDefault(p => p.Id == b.ProductId);
            return MapToDto(b, prod?.Name ?? "Unidentified item", prod?.Sku ?? "---");
        }).ToList();
    }


    public async Task<List<BatchDto>> Handle(GetExpiringBatchesQuery request, CancellationToken cancellationToken)
    {
        var batches = await _batchRepository.GetAllAsync();
        var products = await _productRepository.GetAllAsync(includeInactive: true);
        
        DateTime thresholdDate = DateTime.UtcNow.AddDays(request.DaysThreshold);

        return batches
            .Where(b => b.RemainingQuantity > 0 && b.ExpirationDate <= thresholdDate)
            .OrderBy(b => b.ExpirationDate)
            .Select(b => {
                var prod = products.FirstOrDefault(p => p.Id == b.ProductId);
                return MapToDto(b, prod?.Name ?? "Unidentified item", prod?.Sku ?? "---");
            }).ToList();
    }

    public async Task<BatchDto> Handle(GetBatchByIdQuery request, CancellationToken cancellationToken)
    {
        var batch = await _batchRepository.GetByIdAsync(request.Id) 
                    ?? throw new KeyNotFoundException("No record found.");
        
        var prod = await _productRepository.GetByIdAsync(batch.ProductId);
        return MapToDto(batch, prod?.Name ?? "Unidentified item", prod?.Sku ?? "---");
    }

    public async Task Handle(ReduceBatchStockCommand request, CancellationToken cancellationToken)
    {
        var batch = await _batchRepository.GetByIdAsync(request.BatchId) 
                    ?? throw new KeyNotFoundException("No record found.");

        if (batch.Version != request.Version)
            throw new InvalidOperationException("Conflict of parallelism: the party's remains have already been modified by another operation.");

        batch.ReduceStock(request.QuantityToReduce);
        batch.SetUpdated(request.CurrentUserId);

        await _batchRepository.UpdateAsync(batch);
    }

    private static BatchDto MapToDto(Domain.Entities.Batch b, string productName, string productSku) => new()
    {
        Id = b.Id,
        ProductId = b.ProductId,
        ProductName = productName,
        ProductSku = productSku,
        QuantityReceived = b.QuantityReceived,
        RemainingQuantity = b.RemainingQuantity,
        PurchasePrice = b.PurchasePrice,
        ExpirationDate = b.ExpirationDate,
        SupplyDate = b.CreatedAt,
        Version = b.Version
    };
}