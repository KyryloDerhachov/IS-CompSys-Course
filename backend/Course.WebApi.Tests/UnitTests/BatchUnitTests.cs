using System;
using Course.WebApi.Domain.Entities;
using Xunit;

namespace Course.WebApi.Tests.UnitTests;

public class BatchUnitTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldInitializeCorrectly()
    {
        var productId = Guid.NewGuid();
        var supplyItemId = Guid.NewGuid();
        var quantity = 100m;
        var price = 15.50m;
        var expirationDate = DateTime.UtcNow.AddDays(30);
        var userId = Guid.NewGuid();
        var batch = new Batch(productId, supplyItemId, quantity, price, expirationDate, userId);

        Assert.NotEqual(Guid.Empty, batch.Id);
        Assert.Equal(productId, batch.ProductId);
        Assert.Equal(supplyItemId, batch.SupplyItemId);
        Assert.Equal(quantity, batch.QuantityReceived);
        Assert.Equal(quantity, batch.RemainingQuantity);
        Assert.Equal(price, batch.PurchasePrice);
        Assert.Equal(expirationDate.Date, batch.ExpirationDate);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Constructor_InvalidQuantity_ShouldThrowArgumentException(decimal invalidQuantity)
    {
        Assert.Throws<ArgumentException>(() =>
            new Batch(Guid.NewGuid(), Guid.NewGuid(), invalidQuantity, 10m, DateTime.UtcNow.AddDays(10), Guid.NewGuid())
        );
    }

    [Fact]
    public void Constructor_PastExpirationDate_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new Batch(Guid.NewGuid(), Guid.NewGuid(), 50m, 10m, DateTime.UtcNow.AddDays(-1), Guid.NewGuid())
        );
    }

    [Fact]
    public void ReduceStock_ValidQuantity_ShouldDecreaseRemainingQuantity()
    {
        var batch = new Batch(Guid.NewGuid(), Guid.NewGuid(), 100m, 10m, DateTime.UtcNow.AddDays(10), Guid.NewGuid());
        batch.ReduceStock(30m);
        Assert.Equal(70m, batch.RemainingQuantity);
    }

    [Fact]
    public void ReduceStock_QuantityExceedsRemaining_ShouldThrowInvalidOperationException()
    {
        var batch = new Batch(Guid.NewGuid(), Guid.NewGuid(), 50m, 10m, DateTime.UtcNow.AddDays(10), Guid.NewGuid());
        Assert.Throws<InvalidOperationException>(() => batch.ReduceStock(60m));
    }

    [Fact]
    public void AdjustStock_ValidNewQuantity_ShouldUpdateRemainingQuantity()
    {
        var batch = new Batch(Guid.NewGuid(), Guid.NewGuid(), 100m, 10m, DateTime.UtcNow.AddDays(10), Guid.NewGuid());
        batch.AdjustStock(80m, Guid.NewGuid());
        Assert.Equal(80m, batch.RemainingQuantity);
    }

    [Fact]
    public void AdjustStock_NewQuantityExceedsReceived_ShouldThrowInvalidOperationException()
    {
        var batch = new Batch(Guid.NewGuid(), Guid.NewGuid(), 100m, 10m, DateTime.UtcNow.AddDays(10), Guid.NewGuid());
        Assert.Throws<InvalidOperationException>(() => batch.AdjustStock(120m, Guid.NewGuid()));
    }

    [Fact]
    public void ReceiveRefund_ActiveBatch_ShouldIncreaseQuantityAndReturnTrue()
    {
        // Arrange
        var batch = new Batch(Guid.NewGuid(), Guid.NewGuid(), 100m, 10m, DateTime.UtcNow.AddDays(10), Guid.NewGuid());
        batch.ReduceStock(40m); 
        var result = batch.ReceiveRefund(10m);
        Assert.True(result);
        Assert.Equal(70m, batch.RemainingQuantity);
    }
}