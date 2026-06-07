using Course.WebApi.Application.Interfaces;
using Course.WebApi.Domain.Entities;
using Course.WebApi.Features.Sales.Commands;
using MediatR;

namespace Course.WebApi.Features.Sales.Handlers;

public class SaleCommandHandler : IRequestHandler<CreateReceiptCommand, Guid>
{
    private readonly IReceiptRepository _receiptRepository;
    private readonly IBatchRepository _batchRepository;
    private readonly IProductRepository _productRepository;

    public SaleCommandHandler(IReceiptRepository receiptRepository, IBatchRepository batchRepository, IProductRepository productRepository)
    {
        _receiptRepository = receiptRepository;
        _batchRepository = batchRepository;
        _productRepository = productRepository;
    }

    public async Task<Guid> Handle(CreateReceiptCommand request, CancellationToken cancellationToken)
    {
        var productBatchesMap = new Dictionary<Guid, List<Batch>>();

        foreach (var saleItem in request.Items)
        {
            var product = await _productRepository.GetByIdAsync(saleItem.ProductId) 
                          ?? throw new KeyNotFoundException($"The item with ID {saleItem.ProductId} was not found.");
            
            var batches = (await _batchRepository.GetActiveBatchesByProductAsync(saleItem.ProductId)).ToList();
            
            var totalAvailable = batches.Sum(b => b.RemainingQuantity);

            if (totalAvailable < saleItem.Quantity)
            {
                throw new InvalidOperationException(
                    $"We are unable to process your receipt. We are out of stock of '{product.Name}'.. " +
                    $"Ordered: {saleItem.Quantity}, in stock: {totalAvailable}."
                );
            }

            productBatchesMap[saleItem.ProductId] = batches;
        }
        
        var receipt = new Receipt(request.CurrentUserId);

        foreach (var saleItem in request.Items)
        {
            var product = await _productRepository.GetByIdAsync(saleItem.ProductId);
            var batches = productBatchesMap[saleItem.ProductId];

            var soldItems = product!.Sell(saleItem.Quantity, saleItem.Price, batches, receipt.Id, request.CurrentUserId);

            foreach (var item in soldItems)
            {
                receipt.AddDomainItem(item);

                var modifiedBatch = batches.First(b => b.Id == item.BatchId);
                await _batchRepository.UpdateAsync(modifiedBatch);
            }
        }

        await _receiptRepository.AddAsync(receipt);
        await _receiptRepository.SaveChangesAsync(cancellationToken); 

        return receipt.Id;
    }
}