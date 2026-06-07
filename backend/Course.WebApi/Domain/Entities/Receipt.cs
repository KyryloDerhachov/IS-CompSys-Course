using Course.WebApi.Domain.Common;

namespace Course.WebApi.Domain.Entities;


public class Receipt : AuditableEntity<Guid>
{
    public string ReceiptNumber { get; private set; } = string.Empty;
    public DateTime SaleDate { get; private set; }
    public decimal TotalAmount { get; private set; }

    private readonly List<ReceiptItem> _items = new();
    public IReadOnlyCollection<ReceiptItem> Items => _items.AsReadOnly();

    public Receipt() { }

    public Receipt(Guid? userId)
    {
        Id = Guid.NewGuid();
        SaleDate = DateTime.UtcNow;
        var randomSuffix = Guid.NewGuid().ToString()[..6].ToUpper();
        ReceiptNumber = $"REC-{SaleDate:yyyyMMddHHmmss}-{randomSuffix}";
        
        SetCreated(userId);
    }

    public void ValidateReturnEligibility(Guid productId, decimal quantityToReturn, IEnumerable<ReturnTransaction> pastReturns)
    {
        if (quantityToReturn <= 0) 
            throw new ArgumentException("The return value must be greater than zero.");

        
        decimal totalPurchased = _items.Where(i => i.ProductId == productId).Sum(i => i.Quantity);
        if (totalPurchased == 0)
            throw new InvalidOperationException("This item was not purchased with the receipt shown.");


        decimal totalAlreadyReturned = pastReturns
            .SelectMany(r => r.Items)
            .Where(i => i.ProductId == productId)
            .Sum(i => i.Quantity);


        if (totalAlreadyReturned + quantityToReturn > totalPurchased)
        {
            throw new InvalidOperationException(
                $"It is not possible to return {quantityToReturn} items. The limit for purchased items has been exceeded.. " +
                $"Purchased: {totalPurchased}, has already been returned: {totalAlreadyReturned}");
        }
    }

    public void AddDomainItem(ReceiptItem item)
    {
        _items.Add(item);
        TotalAmount += item.Total;
    }
}

public class ReceiptItem
{
    public Guid Id { get; private set; }
    public Guid ReceiptId { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid BatchId { get; private set; } 
    public decimal Quantity { get; private set; }
    public decimal Price { get; private set; }
    public decimal Total => Quantity * Price;

    public ReceiptItem(Guid receiptId, Guid productId, Guid batchId, decimal quantity, decimal price)
    {
        Id = Guid.NewGuid();
        ReceiptId = receiptId;
        ProductId = productId;
        BatchId = batchId;
        Quantity = quantity;
        Price = price;
    }
}