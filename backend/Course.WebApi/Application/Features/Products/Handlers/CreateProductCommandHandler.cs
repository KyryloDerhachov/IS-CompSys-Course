using Course.WebApi.Domain.Entities;
using Course.WebApi.Application.Interfaces;
using Course.WebApi.Features.Products.Commands;
using MediatR;

namespace Course.WebApi.Features.Products.Handlers;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IProductRepository _productRepository;

    public CreateProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var existing = await _productRepository.GetBySkuAsync(request.Sku);
        if (existing != null)
            throw new InvalidOperationException($"The product with SKU '{request.Sku}' already exists.");

        var product = new Product(
        sku: request.Sku,
        barcode: request.Barcode,
        name: request.Name,
        classId: request.ClassId,
        categoryId: request.CategoryId,
        unit: request.Unit,
        defaultPurchasePrice: request.DefaultPurchasePrice,
        salePrice: request.DefaultSalePrice,
        attributes: request.Attributes,
        userId: request.CurrentUserId
    );

        await _productRepository.AddAsync(product);
        return product.Id;
    }
}