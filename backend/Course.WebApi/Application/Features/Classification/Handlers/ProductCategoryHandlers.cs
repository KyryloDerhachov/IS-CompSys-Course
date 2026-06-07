using Course.WebApi.Domain.Entities;
using Course.WebApi.Application.Interfaces;
using Course.WebApi.Features.Classification.Commands;
using Course.WebApi.Features.Classification.DTOs;
using Course.WebApi.Features.Classification.Queries;
using MediatR;

namespace Course.WebApi.Features.Classification.Handlers;

public class ProductCategoryHandlers :
    IRequestHandler<CreateProductCategoryCommand, Guid>,
    IRequestHandler<UpdateProductCategoryDescriptionCommand>,
    IRequestHandler<DeactivateProductCategoryCommand>,
    IRequestHandler<ActivateProductCategoryCommand>,
    IRequestHandler<GetProductCategoriesQuery, List<ProductCategoryDto>>,
    IRequestHandler<GetProductCategoryByIdQuery, ProductCategoryDto>
{
    private readonly IProductCategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;

    public ProductCategoryHandlers(IProductCategoryRepository categoryRepository, IProductRepository productRepository)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
    }

    public async Task<Guid> Handle(CreateProductCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new ProductCategory(request.ParentId, request.Code, request.Name, request.Description, request.CurrentUserId);
        await _categoryRepository.AddAsync(category);
        return category.Id;
    }

    public async Task Handle(UpdateProductCategoryDescriptionCommand request, CancellationToken cancellationToken)
    {
        var entity = await _categoryRepository.GetByIdAsync(request.Id) ?? throw new KeyNotFoundException("Category not found");
        if (entity.Version != request.Version) throw new InvalidOperationException("Data version conflict.");

        entity.UpdateDescription(request.Description, request.CurrentUserId);
        await _categoryRepository.UpdateAsync(entity);
    }

    public async Task Handle(DeactivateProductCategoryCommand request, CancellationToken cancellationToken)
    {
        var rootCategory = await _categoryRepository.GetByIdAsync(request.Id) ?? throw new KeyNotFoundException("Category not found");
        if (rootCategory.Version != request.Version) throw new InvalidOperationException("Data version conflict.");

        var allCategories = (await _categoryRepository.GetAllAsync(includeInactive: true)).ToList();
        var allProducts = (await _productRepository.GetAllAsync(includeInactive: false)).ToList();

        
        var categoriesToDeactivate = new List<ProductCategory>();
        PopulateSubcategories(rootCategory, allCategories, categoriesToDeactivate);


        foreach (var category in categoriesToDeactivate)
        {
            category.Deactivate(request.CurrentUserId);
            await _categoryRepository.UpdateAsync(category);
        }

        var targetCategoryIds = categoriesToDeactivate.Select(c => c.Id).ToHashSet();
        var productsToDeactivate = allProducts.Where(p => targetCategoryIds.Contains(p.CategoryId));

        foreach (var product in productsToDeactivate)
        {
            product.Deactivate(request.CurrentUserId);
            await _productRepository.UpdateAsync(product);
        }
    }

    public async Task Handle(ActivateProductCategoryCommand request, CancellationToken cancellationToken)
    {
        var rootCategory = await _categoryRepository.GetByIdAsync(request.Id) ?? throw new KeyNotFoundException("Category not found");
        if (rootCategory.Version != request.Version) throw new InvalidOperationException("Data version conflict.");

        var allCategories = (await _categoryRepository.GetAllAsync(includeInactive: true)).ToList();
        
        var categoriesToActivate = new List<ProductCategory>();
        PopulateSubcategoriesForActivation(rootCategory, allCategories, categoriesToActivate);

        foreach (var category in categoriesToActivate)
        {
            category.Activate(request.CurrentUserId);
            await _categoryRepository.UpdateAsync(category);
        }
    }
    private void PopulateSubcategories(ProductCategory current, List<ProductCategory> allCategories, List<ProductCategory> result)
    {
        result.Add(current);
        var subcategories = allCategories.Where(c => c.ParentId == current.Id && c.IsActive);
        foreach (var sub in subcategories)
        {
            PopulateSubcategories(sub, allCategories, result);
        }
    }
    private void PopulateSubcategoriesForActivation(ProductCategory current, List<ProductCategory> allCategories, List<ProductCategory> result)
{
    result.Add(current);
    var subcategories = allCategories.Where(c => c.ParentId == current.Id);
    foreach (var sub in subcategories)
    {
        PopulateSubcategoriesForActivation(sub, allCategories, result);
    }
}

    public async Task<List<ProductCategoryDto>> Handle(GetProductCategoriesQuery request, CancellationToken cancellationToken)
    {
        var list = await _categoryRepository.GetAllAsync(request.IncludeInactive);
        return list.Select(c => new ProductCategoryDto(c.Id, c.ParentId, c.Code, c.Name, c.Description, c.IsActive, c.Version)).ToList();
    }

    public async Task<ProductCategoryDto> Handle(GetProductCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var c = await _categoryRepository.GetByIdAsync(request.Id) ?? throw new KeyNotFoundException("Category not found");
        return new ProductCategoryDto(c.Id, c.ParentId, c.Code, c.Name, c.Description, c.IsActive, c.Version);
    }
}