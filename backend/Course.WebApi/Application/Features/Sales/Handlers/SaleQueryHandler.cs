using Course.WebApi.Application.Interfaces;
using Course.WebApi.Features.Sales.DTOs;
using Course.WebApi.Features.Sales.Queries;
using MediatR;

namespace Course.WebApi.Features.Sales.Handlers;

public class SaleQueryHandler : 
    IRequestHandler<GetReceiptByIdQuery, ReceiptDto>,
    IRequestHandler<GetReceiptsHistoryQuery, List<ReceiptDto>>
{
    private readonly IReceiptRepository _receiptRepository;
    private readonly IProductRepository _productRepository;

    public SaleQueryHandler(IReceiptRepository receiptRepository, IProductRepository productRepository)
    {
        _receiptRepository = receiptRepository;
        _productRepository = productRepository;
    }

    public async Task<ReceiptDto> Handle(GetReceiptByIdQuery request, CancellationToken cancellationToken)
    {
        var receipt = await _receiptRepository.GetByIdAsync(request.Id) ?? throw new KeyNotFoundException("The receipt was not found.");
        return await MapToDto(receipt);
    }

    public async Task<List<ReceiptDto>> Handle(GetReceiptsHistoryQuery request, CancellationToken cancellationToken)
    {
        var receipts = await _receiptRepository.GetAllAsync();
        var dtos = new List<ReceiptDto>();
        foreach (var r in receipts)
        {
            dtos.Add(await MapToDto(r));
        }
        return dtos.OrderByDescending(r => r.SaleDate).ToList();
    }

    private async Task<ReceiptDto> MapToDto(Domain.Entities.Receipt r)
    {
        var itemsDto = new List<ReceiptItemDto>();
        var products = await _productRepository.GetAllAsync(includeInactive: true);

        foreach (var item in r.Items)
        {
            var p = products.FirstOrDefault(prod => prod.Id == item.ProductId);
            itemsDto.Add(new ReceiptItemDto(item.ProductId, p?.Name ?? "Unidentified item", item.Quantity, item.Price, item.Total));
        }

        return new ReceiptDto(r.Id, r.ReceiptNumber, r.SaleDate, r.TotalAmount, itemsDto);
    }
}