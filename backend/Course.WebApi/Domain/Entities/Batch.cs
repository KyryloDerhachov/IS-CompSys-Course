using System;
using Course.WebApi.Domain.Common;

namespace Course.WebApi.Domain.Entities;

public class Batch : AuditableEntity<Guid>
{

    public Guid ProductId { get; private set; }
    public Guid SupplyItemId { get; private set; }
    public decimal QuantityReceived { get; private set; }
    public decimal RemainingQuantity { get; private set; }
    public decimal PurchasePrice { get; private set; }
    public DateTime ExpirationDate { get; private set; }

    public bool IsExpired => ExpirationDate < DateTime.UtcNow;

    public Batch() { }

    public Batch(Guid productId, Guid supplyItemId, decimal quantity, decimal purchasePrice, DateTime expirationDate, Guid? userId)
    {
        if (quantity <= 0)
            throw new ArgumentException("The initial quantity of goods in the batch must be greater than zero.");
        
        if (purchasePrice <= 0)
            throw new ArgumentException("The purchase price for the batch must be greater than zero.");
        
        if (expirationDate <= DateTime.UtcNow)
            throw new ArgumentException("The expiration date of a new batch cannot be in the past.");

        Id = Guid.NewGuid();
        ProductId = productId;
        SupplyItemId = supplyItemId;
        QuantityReceived = quantity;
        RemainingQuantity = quantity; 
        PurchasePrice = purchasePrice;
        ExpirationDate = expirationDate.Date; 
        
        SetCreated(userId);
    }

    public void ReduceStock(decimal quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("The quantity to be written off must be greater than zero.");


        if (quantity > RemainingQuantity)
        {
            throw new InvalidOperationException(
                $"There is not enough stock in batch {Id}. Requested for write-off: {quantity}, available: {RemainingQuantity}.");
        }

        RemainingQuantity -= quantity;
    }


    public void AdjustStock(decimal newRemainingQuantity, Guid? userId)
    {
        if (newRemainingQuantity < 0)
            throw new ArgumentException("The remaining inventory after adjustments cannot be less than zero.");
        
        if (newRemainingQuantity > QuantityReceived)
            throw new InvalidOperationException("The current balance cannot exceed the initial quantity of goods received.");

        RemainingQuantity = newRemainingQuantity;
        SetUpdated(userId);
    }
    public bool ReceiveRefund(decimal quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("The quantity to be returned must be greater than zero.");

        
        if (IsExpired)
        {
            
            return false; 
        }

        RemainingQuantity += quantity;
        return true;
    }
}