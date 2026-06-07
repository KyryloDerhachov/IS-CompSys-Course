using Course.WebApi.Application.Interfaces;
using Course.WebApi.Features.Products.DTOs;
using Course.WebApi.Features.Products.Queries;
using MediatR;

namespace Course.WebApi.Features.Products.Handlers;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductCategoryRepository _categoryRepository;
    private readonly IProductClassRepository _classRepository;

    public GetProductsQueryHandler(
        IProductRepository productRepository,
        IProductCategoryRepository categoryRepository,
        IProductClassRepository classRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _classRepository = classRepository;
    }

    public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllAsync(request.IncludeInactive);
        var categories = await _categoryRepository.GetAllAsync();
        var classes = await _classRepository.GetAllAsync();

        return products.Select(p => 
        {
            var category = categories.FirstOrDefault(c => c.Id == p.CategoryId);
            var productClass = classes.FirstOrDefault(cl => cl.Id == p.ClassId);

            return new ProductDto
            {
                Id = p.Id,
                Sku = p.Sku,
                Barcode = p.Barcode,
                Name = p.Name,
                ClassId = p.ClassId,
                ClassName = productClass?.Name ?? "Unknown class",
                CategoryId = p.CategoryId,
                CategoryName = category?.Name ?? "Uncategorized",
                Unit = p.Unit,
                DefaultPurchasePrice = p.DefaultPurchasePrice,
                DefaultSalePrice = p.DefaultSalePrice,
                IsActive = p.IsActive,
                Version = p.Version,
                Attributes = p.Attributes

            };
        }).ToList();
    }
}