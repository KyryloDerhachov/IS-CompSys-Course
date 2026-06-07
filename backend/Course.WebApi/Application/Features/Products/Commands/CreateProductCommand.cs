using MediatR;

namespace Course.WebApi.Features.Products.Commands;

public record CreateProductCommand(
    string Sku,
    string Barcode,
    string Name,
    Guid ClassId,
    Guid CategoryId,
    string Unit,
    int? DefaultShelfLifeDays,
    decimal DefaultPurchasePrice,
    decimal DefaultSalePrice,
    Dictionary<string, string>? Attributes,
    Guid? CurrentUserId 
) : IRequest<Guid>;