namespace Course.WebApi.Features.Supplies.Handlers;

using Course.WebApi.Domain.Entities;
using Course.WebApi.Application.Interfaces;
using Course.WebApi.Features.Supplies.Commands;
using MediatR;

public class SupplyCommandHandler : 
    IRequestHandler<CreateSupplyCommand, Guid>,
    IRequestHandler<PostSupplyCommand>
{
    private readonly ISupplyRepository _supplyRepository;
    private readonly IBatchRepository _batchRepository;
    private readonly IProductRepository _productRepository;

    public SupplyCommandHandler(ISupplyRepository supplyRepository, IBatchRepository batchRepository, IProductRepository productRepository)
    {
        _supplyRepository = supplyRepository;
        _batchRepository = batchRepository;
        _productRepository = productRepository;
    }

    public async Task<Guid> Handle(CreateSupplyCommand request, CancellationToken cancellationToken)
    {
        var supply = new Supply(request.SupplierName, request.SupplyDate, request.CurrentUserId);
        
        foreach (var item in request.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId) 
                          ?? throw new KeyNotFoundException($"The item with ID {item.ProductId} was not found.");
            
            supply.AddItem(product, item.Quantity, item.PurchasePrice, item.ShelfLifeDays);
        }

        await _supplyRepository.AddAsync(supply);
        return supply.Id;
    }

    public async Task Handle(PostSupplyCommand request, CancellationToken cancellationToken)
    {
        var supply = await _supplyRepository.GetByIdAsync(request.SupplyId) 
                     ?? throw new KeyNotFoundException("The delivery document was not found.");

        supply.Post(request.CurrentUserId);

        foreach (var item in supply.Items)
        {
            DateTime expirationDate = DateTime.UtcNow.Date.AddDays(item.ShelfLifeDays);


            var batch = new Batch(
                productId: item.ProductId,
                supplyItemId: item.Id,
                quantity: item.Quantity,
                purchasePrice: item.PurchasePrice,
                expirationDate: expirationDate,
                userId: request.CurrentUserId
            );

            await _batchRepository.AddAsync(batch);
        }

        await _supplyRepository.UpdateAsync(supply);
    }
}