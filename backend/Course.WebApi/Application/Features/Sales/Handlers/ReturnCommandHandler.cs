using Course.WebApi.Domain.Entities;
using Course.WebApi.Application.Interfaces;
using Course.WebApi.Features.Sales.Commands;
using MediatR;

namespace Course.WebApi.Features.Sales.Handlers;

public class ReturnCommandHandler : IRequestHandler<ProcessReturnCommand, Guid>
{
    private readonly IReceiptRepository _receiptRepository;
    private readonly IReturnRepository _returnRepository;
    private readonly IBatchRepository _batchRepository;

    public ReturnCommandHandler(
        IReceiptRepository receiptRepository, 
        IReturnRepository returnRepository, 
        IBatchRepository batchRepository)
    {
        _receiptRepository = receiptRepository;
        _returnRepository = returnRepository;
        _batchRepository = batchRepository;
    }

    public async Task<Guid> Handle(ProcessReturnCommand request, CancellationToken cancellationToken)
    {
        var receipt = await _receiptRepository.GetByIdAsync(request.ReceiptId)
                    ?? throw new KeyNotFoundException("The original receipt was not found.");

        var pastReturns = (await _returnRepository.GetByReceiptIdAsync(request.ReceiptId)).ToList();
        

        var returnTx = new ReturnTransaction(request.ReceiptId, request.CurrentUserId);

        foreach (var returnInput in request.Items)
        {

            receipt.ValidateReturnEligibility(returnInput.ProductId, returnInput.Quantity, pastReturns);


            var receiptLines = receipt.Items
                .Where(i => i.ProductId == returnInput.ProductId)
                .OrderByDescending(i => i.Quantity)
                .ToList();

            decimal quantityToRefund = returnInput.Quantity;

            foreach (var line in receiptLines)
            {
                if (quantityToRefund <= 0) break;

                decimal amountFromThisLine = Math.Min(line.Quantity, quantityToRefund);
                var batch = await _batchRepository.GetByIdAsync(line.BatchId);
                
                bool returnedToStock = false;

                if (batch != null)
                {
                    returnedToStock = batch.ReceiveRefund(amountFromThisLine);
                    batch.SetUpdated(request.CurrentUserId);
                    
                    await _batchRepository.UpdateAsync(batch);
                }

                returnTx.AddItem(line.ProductId, line.BatchId, amountFromThisLine, line.Price, returnedToStock);
                quantityToRefund -= amountFromThisLine;
            }
        }

        await _returnRepository.AddAsync(returnTx);
        return returnTx.Id;
    }
}