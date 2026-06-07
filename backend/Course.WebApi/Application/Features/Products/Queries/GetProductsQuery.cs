using Course.WebApi.Features.Products.DTOs;
using MediatR;

namespace Course.WebApi.Features.Products.Queries;

public record GetProductsQuery(bool IncludeInactive = false) : IRequest<List<ProductDto>>;