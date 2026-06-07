using MediatR;

namespace Course.WebApi.Features.Products.Commands;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    Guid ClassId,
    Guid CategoryId,
    string Unit,
    int Version,
    Dictionary<string, string>? Attributes,
    Guid? CurrentUserId
) : IRequest;