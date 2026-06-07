using Course.WebApi.Application.Interfaces;
using Course.WebApi.Features.Products.Commands;
using MediatR;

namespace Course.WebApi.Features.Products.Handlers;

public class DeactivateProductCommandHandler : IRequestHandler<DeactivateProductCommand>
{
    private readonly IProductRepository _productRepository;

    public DeactivateProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task Handle(DeactivateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id);
        if (product == null)
        {
            throw new KeyNotFoundException($"The item with ID {request.Id} was not found.");
        }

        if (product.Version != request.Version)
        {
            throw new InvalidOperationException(
                "Version conflict: The product details have been modified by another user. Please refresh the page before deactivating.");
        }

        product.Deactivate(request.CurrentUserId);

        await _productRepository.UpdateAsync(product);
    }
}