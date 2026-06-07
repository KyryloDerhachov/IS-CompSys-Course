using MediatR;

namespace Course.WebApi.Features.Products.Commands;

public record DeactivateProductCommand(
    Guid Id, 
    int Version,
    Guid? CurrentUserId
) : IRequest;