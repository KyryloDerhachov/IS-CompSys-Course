using System;
using System.Collections.Generic;
using System.Linq;
using Course.WebApi.Domain.Common;

namespace Course.WebApi.Domain.Entities;

public class Product : AuditableEntity<Guid>
{

    public string Sku { get; private set; } = string.Empty;
    public string Barcode { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public Guid ClassId { get; private set; }
    public Guid CategoryId { get; private set; }
    public string Unit { get; private set; } = "шт";
    public decimal DefaultPurchasePrice { get; private set; }
    public decimal DefaultSalePrice { get; private set; }
    public bool IsActive { get; private set; } = true;


    public Dictionary<string, string> Attributes { get; private set; } = new();


    public Product() { }


public Product(
        string sku, 
        string barcode, 
        string name, 
        Guid classId, 
        Guid categoryId, 
        string unit,
        decimal defaultPurchasePrice, 
        decimal salePrice, 
        Dictionary<string, string>? attributes, 
        Guid? userId)
    {
        if (string.IsNullOrWhiteSpace(name)) 
            throw new ArgumentException("The product name cannot be left blank.");
        if (salePrice <= 0) 
            throw new ArgumentException("The retail price must be greater than zero.");

        Id = Guid.NewGuid();
        Sku = sku.Trim().ToUpper();
        Barcode = barcode.Trim();
        Name = name.Trim();
        ClassId = classId;
        CategoryId = categoryId;
        Unit = string.IsNullOrWhiteSpace(unit) ? "шт" : unit.Trim();
        DefaultPurchasePrice = defaultPurchasePrice;
        DefaultSalePrice = salePrice;
        IsActive = true;
        
        Attributes = attributes ?? new Dictionary<string, string>();
        
        SetCreated(userId);
    }

    public void UpdateDetails(string name, Guid classId, Guid categoryId, string unit, Dictionary<string, string>? attributes, Guid? userId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("The product name cannot be left blank.");

        Name = name.Trim();
        ClassId = classId;
        CategoryId = categoryId;
        Unit = string.IsNullOrWhiteSpace(unit) ? "шт" : unit.Trim();
        
        Attributes = attributes ?? new Dictionary<string, string>();

        SetUpdated(userId);
    }


    public void Deactivate(Guid? Id)
    {
        if (!IsActive) return; 
        
        IsActive = false;
        SetUpdated(Id);
    }

    public void Activate(Guid? Id)
    {
        if (IsActive) return;

        IsActive = true;
        SetUpdated(Id);
    }


    public List<ReceiptItem> Sell(decimal quantity, decimal price, List<Batch> productBatches, Guid receiptId, Guid? userId)
    {
        if (!IsActive)
            throw new InvalidOperationException($"The product '{Name}' (SKU: {Sku}) has been deactivated. It is not available for purchase..");

        if (quantity <= 0)
            throw new ArgumentException("The quantity of goods for sale must be greater than zero.");

        var validBatches = productBatches
            .Where(b => b.RemainingQuantity > 0 && b.ExpirationDate > DateTime.UtcNow)
            .OrderBy(b => b.ExpirationDate) 
            .ToList();


        decimal totalAvailable = validBatches.Sum(b => b.RemainingQuantity);
        if (totalAvailable < quantity)
        {
            throw new InvalidOperationException(
                $"We are out of stock of '{Name}'. Requested: {quantity} {Unit}, in stock: {totalAvailable} {Unit}.");
        }

        var generatedReceiptItems = new List<ReceiptItem>();
        decimal quantityToSettle = quantity;


        foreach (var batch in validBatches)
        {
            if (quantityToSettle <= 0) break;

            decimal amountFromThisBatch = Math.Min(batch.RemainingQuantity, quantityToSettle);

            batch.ReduceStock(amountFromThisBatch);
            batch.SetUpdated(userId);


            var receiptItem = new ReceiptItem(receiptId, this.Id, batch.Id, amountFromThisBatch, price);
            generatedReceiptItems.Add(receiptItem);

            quantityToSettle -= amountFromThisBatch;
        }

        SetUpdated(userId);
        return generatedReceiptItems;
    }
}