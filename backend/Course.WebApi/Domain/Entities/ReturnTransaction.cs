using System;
using System.Collections.Generic;
using Course.WebApi.Domain.Common;

namespace Course.WebApi.Domain.Entities;

public class ReturnTransaction : AuditableEntity<Guid>
{
    public Guid ReceiptId { get; private set; }
    public string ReturnNumber { get; private set; } = string.Empty;
    public DateTime ReturnDate { get; private set; }
    public decimal TotalRefundAmount { get; private set; }

    private readonly List<ReturnItem> _items = new();
    public IReadOnlyCollection<ReturnItem> Items => _items.AsReadOnly();

    public ReturnTransaction() { }

    public ReturnTransaction(Guid receiptId, Guid? userId)
    {
        Id = Guid.NewGuid();
        ReceiptId = receiptId;
        ReturnDate = DateTime.UtcNow;
        ReturnNumber = $"RET-{ReturnDate:yyyyMMddHHmmss}";
        SetCreated(userId);
    }

    public void AddItem(Guid productId, Guid batchId, decimal quantity, decimal price, bool returnedToStock)
    {
        var item = new ReturnItem(Id, productId, batchId, quantity, price, returnedToStock);
        _items.Add(item);
        TotalRefundAmount += item.Total;
    }
}

public class ReturnItem
{
    public Guid Id { get; private set; }
    public Guid ReturnTransactionId { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid BatchId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal Price { get; private set; }
    public bool ReturnedToStock { get; private set; }
    public decimal Total => Quantity * Price;

    public ReturnItem(Guid returnTransactionId, Guid productId, Guid batchId, decimal quantity, decimal price, bool returnedToStock)
    {
        Id = Guid.NewGuid();
        ReturnTransactionId = returnTransactionId;
        ProductId = productId;
        BatchId = batchId;
        Quantity = quantity;
        Price = price;
        ReturnedToStock = returnedToStock;
    }
}