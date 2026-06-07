using Course.WebApi.Features.Products.DTOs;
using MediatR;

namespace Course.WebApi.Features.Products.Queries;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;