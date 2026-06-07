using Course.WebApi.Features.Batches.DTOs;
using MediatR;

namespace Course.WebApi.Features.Batches.Queries;

public record GetBatchesQuery(Guid? ProductId = null) : IRequest<List<BatchDto>>;

public record GetExpiringBatchesQuery(int DaysThreshold = 7) : IRequest<List<BatchDto>>;

public record GetBatchByIdQuery(Guid Id) : IRequest<BatchDto>;