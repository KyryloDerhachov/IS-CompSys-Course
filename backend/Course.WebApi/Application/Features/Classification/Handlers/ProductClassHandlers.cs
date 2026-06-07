using Course.WebApi.Domain.Entities;
using Course.WebApi.Application.Interfaces;
using Course.WebApi.Features.Classification.Commands;
using Course.WebApi.Features.Classification.DTOs;
using Course.WebApi.Features.Classification.Queries;
using MediatR;

namespace Course.WebApi.Features.Classification.Handlers;

public class ProductClassHandlers : 
    IRequestHandler<CreateProductClassCommand, Guid>,
    IRequestHandler<UpdateProductClassDescriptionCommand>,
    IRequestHandler<DeactivateProductClassCommand>,
    IRequestHandler<ActivateProductClassCommand>,
    IRequestHandler<GetProductClassesQuery, List<ProductClassDto>>,
    IRequestHandler<GetProductClassByIdQuery, ProductClassDto>
{
    private readonly IProductClassRepository _classRepository;
    private readonly IProductRepository _productRepository;

    public ProductClassHandlers(IProductClassRepository classRepository, IProductRepository productRepository)
    {
        _classRepository = classRepository;
        _productRepository = productRepository;
    }

    public async Task<Guid> Handle(CreateProductClassCommand request, CancellationToken cancellationToken)
    {
        var classEntity = new ProductClass(request.Code, request.Name, request.Description, request.CurrentUserId);
        await _classRepository.AddAsync(classEntity);
        return classEntity.Id;
    }

    public async Task Handle(UpdateProductClassDescriptionCommand request, CancellationToken cancellationToken)
    {
        var entity = await _classRepository.GetByIdAsync(request.Id) ?? throw new KeyNotFoundException("Class not found");
        
        if (entity.Version != request.Version) throw new InvalidOperationException("Data version conflict");

        entity.UpdateDescription(request.Description, request.CurrentUserId);
        await _classRepository.UpdateAsync(entity);
    }

    public async Task Handle(DeactivateProductClassCommand request, CancellationToken cancellationToken)
    {
        var classEntity = await _classRepository.GetByIdAsync(request.Id) ?? throw new KeyNotFoundException("Class not found");
        if (classEntity.Version != request.Version) throw new InvalidOperationException("Data version conflict");

  
        classEntity.Deactivate(request.CurrentUserId);
        await _classRepository.UpdateAsync(classEntity);

        var products = await _productRepository.GetAllAsync(includeInactive: false);
        var classProducts = products.Where(p => p.ClassId == request.Id);

        foreach (var product in classProducts)
        {
            product.Deactivate(request.CurrentUserId);
            await _productRepository.UpdateAsync(product);
        }
    }

    public async Task Handle(ActivateProductClassCommand request, CancellationToken cancellationToken)
    {
        var classEntity = await _classRepository.GetByIdAsync(request.Id) ?? throw new KeyNotFoundException("Class not found");
        if (classEntity.Version != request.Version) throw new InvalidOperationException("Data version conflict");
        classEntity.Activate(request.CurrentUserId);
        await _classRepository.UpdateAsync(classEntity);
    }

    public async Task<List<ProductClassDto>> Handle(GetProductClassesQuery request, CancellationToken cancellationToken)
    {
        var list = await _classRepository.GetAllAsync(request.IncludeInactive);
        return list.Select(c => new ProductClassDto(c.Id, c.Code, c.Name, c.Description, c.IsActive, c.Version)).ToList();
    }

    public async Task<ProductClassDto> Handle(GetProductClassByIdQuery request, CancellationToken cancellationToken)
    {
        var c = await _classRepository.GetByIdAsync(request.Id) ?? throw new KeyNotFoundException("Class not found");
        return new ProductClassDto(c.Id, c.Code, c.Name, c.Description, c.IsActive, c.Version);
    }
}