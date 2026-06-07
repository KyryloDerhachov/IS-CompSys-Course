using System;
using System.Collections.Generic;
using System.Linq;
using Course.WebApi.Domain.Entities;
using Xunit;

namespace Course.WebApi.Tests.UnitTests;

public class ProductUnitTests
{
    private Product CreateTestProduct(bool isActive = true)
    {
        var product = new Product(
            sku: "PROD-01",
            barcode: "123456789",
            name: "Test Product",
            classId: Guid.NewGuid(),
            categoryId: Guid.NewGuid(),
            unit: "kg",
            defaultPurchasePrice: 10.00m,
            salePrice: 15.00m,
            attributes: null,
            userId: Guid.NewGuid()
        );

        if (!isActive)
        {
            product.Deactivate(Guid.NewGuid());
        }

        return product;
    }

    private Batch CreateTestBatch(decimal quantity, int daysUntilExpiration)
    {
        return new Batch(
            productId: Guid.NewGuid(),
            supplyItemId: Guid.NewGuid(),
            quantity: quantity,
            purchasePrice: 10.00m,
            expirationDate: DateTime.UtcNow.AddDays(daysUntilExpiration),
            userId: Guid.NewGuid()
        );
    }

    [Fact]
    public void Constructor_EmptyName_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new Product("", "123", " ", Guid.NewGuid(), Guid.NewGuid(), "pcs", 5m, 10m, null, Guid.NewGuid())
        );
    }

    [Fact]
    public void Constructor_NegativeSalePrice_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new Product("SKU", "123", "Name", Guid.NewGuid(), Guid.NewGuid(), "pcs", 5m, -1m, null, Guid.NewGuid())
        );
    }

    [Fact]
    public void Sell_ProductIsDeactivated_ShouldThrowInvalidOperationException()
    {
        var product = CreateTestProduct(isActive: false);
        var batches = new List<Batch> { CreateTestBatch(10m, 5) };

        Assert.Throws<InvalidOperationException>(() =>
            product.Sell(5m, 15m, batches, Guid.NewGuid(), Guid.NewGuid())
        );
    }

    [Fact]
    public void Sell_InsufficentTotalStock_ShouldThrowInvalidOperationException()
    {
        var product = CreateTestProduct();
        var batches = new List<Batch> 
        { 
            CreateTestBatch(5m, 10),
            CreateTestBatch(3m, 15)
        };

        Assert.Throws<InvalidOperationException>(() =>
            product.Sell(10m, 15m, batches, Guid.NewGuid(), Guid.NewGuid())
        );
    }

    [Fact]
    public void Sell_HasExpiredBatches_ShouldIgnoreThemAndThrowException()
    {
        var product = CreateTestProduct();
        
        var expiredBatch = (Batch)Activator.CreateInstance(typeof(Batch), true)!;
        typeof(Batch).GetProperty(nameof(Batch.RemainingQuantity))!.SetValue(expiredBatch, 10m);
        typeof(Batch).GetProperty(nameof(Batch.ExpirationDate))!.SetValue(expiredBatch, DateTime.UtcNow.AddDays(-5));

        var batches = new List<Batch> { expiredBatch };

        Assert.Throws<InvalidOperationException>(() =>
            product.Sell(5m, 15m, batches, Guid.NewGuid(), Guid.NewGuid())
        );
    }

    [Fact]
    public void Sell_SingleBatchCoversQuantity_ShouldReduceStockAndReturnOneReceiptItem()
    {
        var product = CreateTestProduct();
        var batch = CreateTestBatch(50m, 30);
        var batches = new List<Batch> { batch };
        var receiptId = Guid.NewGuid();
        var result = product.Sell(20m, 15m, batches, receiptId, Guid.NewGuid());

        Assert.Single(result);
        Assert.Equal(30m, batch.RemainingQuantity);
        
        var item = result.First();
        Assert.Equal(receiptId, item.ReceiptId);
        Assert.Equal(product.Id, item.ProductId);
        Assert.Equal(batch.Id, item.BatchId);
        Assert.Equal(20m, item.Quantity);
        Assert.Equal(15m, item.Price);
    }

    [Fact]
    public void Sell_MultipleBatchesRequired_ShouldApplyFEFOAndSplitIntoMultipleItems()
    {
        var product = CreateTestProduct();
        var firstExpiringBatch = CreateTestBatch(10m, 5);
        var secondExpiringBatch = CreateTestBatch(20m, 10);
        var lastExpiringBatch = CreateTestBatch(15m, 20); 
        
        var batches = new List<Batch> { lastExpiringBatch, firstExpiringBatch, secondExpiringBatch };

        var result = product.Sell(25m, 15m, batches, Guid.NewGuid(), Guid.NewGuid());


        Assert.Equal(2, result.Count);
        
        Assert.Equal(0m, firstExpiringBatch.RemainingQuantity);
        Assert.Equal(10m, result[0].Quantity);
        Assert.Equal(firstExpiringBatch.Id, result[0].BatchId);

        Assert.Equal(5m, secondExpiringBatch.RemainingQuantity);
        Assert.Equal(15m, result[1].Quantity);
        Assert.Equal(secondExpiringBatch.Id, result[1].BatchId);
        Assert.Equal(15m, lastExpiringBatch.RemainingQuantity);
    }
}