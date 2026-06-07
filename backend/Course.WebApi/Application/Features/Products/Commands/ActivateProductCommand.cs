using MediatR;

namespace Course.WebApi.Features.Products.Commands;

public record ActivateProductCommand(
    Guid Id, 
    int Version,
    Guid? CurrentUserId
) : IRequest;