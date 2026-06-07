using Course.WebApi.Features.Classification.DTOs;
using MediatR;

namespace Course.WebApi.Features.Classification.Queries;

public record GetProductClassesQuery(bool IncludeInactive = false) : IRequest<List<ProductClassDto>>;
public record GetProductClassByIdQuery(Guid Id) : IRequest<ProductClassDto>;

public record GetProductCategoriesQuery(bool IncludeInactive = false) : IRequest<List<ProductCategoryDto>>;
public record GetProductCategoryByIdQuery(Guid Id) : IRequest<ProductCategoryDto>;