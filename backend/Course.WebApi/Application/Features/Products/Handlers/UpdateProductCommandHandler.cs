
using Course.WebApi.Application.Interfaces;
using Course.WebApi.Features.Products.Commands;
using MediatR;

namespace Course.WebApi.Features.Products.Handlers;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
{
    private readonly IProductRepository _productRepository;

    public UpdateProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
{
    var product = await _productRepository.GetByIdAsync(request.Id)
                  ?? throw new KeyNotFoundException($"The item with ID {request.Id} was not found.");

    
    if (product.Version != request.Version)
        throw new InvalidOperationException("Version conflict: This item has already been edited by another user.");

    product.UpdateDetails(
        name: request.Name,
        classId: request.ClassId,
        categoryId: request.CategoryId,
        unit: request.Unit,
        attributes: request.Attributes, 
        userId: request.CurrentUserId
    );

    await _productRepository.UpdateAsync(product);
}
}