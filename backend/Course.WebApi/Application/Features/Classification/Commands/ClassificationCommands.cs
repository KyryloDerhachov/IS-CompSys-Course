using MediatR;

namespace Course.WebApi.Features.Classification.Commands;

public record CreateProductClassCommand(string Code, string Name, string? Description, Guid? CurrentUserId) : IRequest<Guid>;
public record UpdateProductClassDescriptionCommand(Guid Id, string? Description, int Version, Guid? CurrentUserId) : IRequest;
public record DeactivateProductClassCommand(Guid Id, int Version, Guid? CurrentUserId) : IRequest;
public record ActivateProductClassCommand(Guid Id, int Version, Guid? CurrentUserId) : IRequest;

public record CreateProductCategoryCommand(Guid? ParentId, string Code, string Name, string? Description, Guid? CurrentUserId) : IRequest<Guid>;
public record UpdateProductCategoryDescriptionCommand(Guid Id, string? Description, int Version, Guid? CurrentUserId) : IRequest;
public record DeactivateProductCategoryCommand(Guid Id, int Version, Guid? CurrentUserId) : IRequest;
public record ActivateProductCategoryCommand(Guid Id, int Version, Guid? CurrentUserId) : IRequest;