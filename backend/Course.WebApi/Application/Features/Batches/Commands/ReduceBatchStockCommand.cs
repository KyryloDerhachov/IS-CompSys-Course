using MediatR;

namespace Course.WebApi.Features.Batches.Commands;

public record ReduceBatchStockCommand(
    Guid BatchId, 
    decimal QuantityToReduce, 
    int Version, 
    Guid? CurrentUserId
) : IRequest;