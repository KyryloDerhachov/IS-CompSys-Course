using Course.WebApi.Application.Interfaces;
using Course.WebApi.Features.Products.DTOs;
using Course.WebApi.Features.Products.Queries;
using MediatR;

namespace Course.WebApi.Features.Products.Handlers;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductCategoryRepository _categoryRepository;
    private readonly IProductClassRepository _classRepository;

    public GetProductByIdQueryHandler(
        IProductRepository productRepository,
        IProductCategoryRepository categoryRepository,
        IProductClassRepository classRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _classRepository = classRepository;
    }

    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id);
        if (product == null)
        {
            throw new KeyNotFoundException($"The item with ID {request.Id} was not found.");
        }

        var category = await _categoryRepository.GetByIdAsync(product.CategoryId);
        var productClass = await _classRepository.GetByIdAsync(product.ClassId);

        return new ProductDto
        {
            Id = product.Id,
            Sku = product.Sku,
            Barcode = product.Barcode,
            Name = product.Name,
            ClassId = product.ClassId,
            ClassName = productClass?.Name ?? "Unknown class",
            CategoryId = product.CategoryId,
            CategoryName = category?.Name ?? "Uncategorized",
            Unit = product.Unit,
            DefaultPurchasePrice = product.DefaultPurchasePrice,
            DefaultSalePrice = product.DefaultSalePrice,
            IsActive = product.IsActive,
            Version = product.Version,
            Attributes = product.Attributes ?? new Dictionary<string, string>()
        };
    }
}