using System;
using System.Collections.Generic;
using Course.WebApi.Domain.Entities;
using Xunit;

namespace Course.WebApi.Tests.UnitTests;

public class ReceiptUnitTests
{
    [Fact]
    public void Constructor_ShouldInitializeCorrectlyAndGenerateNumber()
    {
        var userId = Guid.NewGuid();

        var receipt = new Receipt(userId);

        Assert.NotEqual(Guid.Empty, receipt.Id);
        Assert.StartsWith("REC-", receipt.ReceiptNumber);
        Assert.Equal(0m, receipt.TotalAmount);
        Assert.Empty(receipt.Items);
    }

    [Fact]
    public void AddDomainItem_ShouldAddItemAndIncreaseTotalAmount()
    {
        var receipt = new Receipt(Guid.NewGuid());
        var productId = Guid.NewGuid();
        var batchId = Guid.NewGuid();
        
        var item1 = new ReceiptItem(receipt.Id, productId, batchId, 2m, 50.00m);
        var item2 = new ReceiptItem(receipt.Id, Guid.NewGuid(), Guid.NewGuid(), 1m, 30.00m);

        receipt.AddDomainItem(item1);
        receipt.AddDomainItem(item2);

        Assert.Equal(2, receipt.Items.Count);
        Assert.Equal(130.00m, receipt.TotalAmount);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void ValidateReturnEligibility_QuantityIsZeroOrLess_ShouldThrowArgumentException(decimal invalidQuantity)
    {
        var receipt = new Receipt(Guid.NewGuid());
        var pastReturns = new List<ReturnTransaction>();

        Assert.Throws<ArgumentException>(() =>
            receipt.ValidateReturnEligibility(Guid.NewGuid(), invalidQuantity, pastReturns)
        );
    }

    [Fact]
    public void ValidateReturnEligibility_ProductNotPurchasedInThisReceipt_ShouldThrowInvalidOperationException()
    {
        var receipt = new Receipt(Guid.NewGuid());
        var purchasedProduct = Guid.NewGuid();
        var nonPurchasedProduct = Guid.NewGuid();
        
        receipt.AddDomainItem(new ReceiptItem(receipt.Id, purchasedProduct, Guid.NewGuid(), 5m, 10m));
        var pastReturns = new List<ReturnTransaction>();

        Assert.Throws<InvalidOperationException>(() =>
            receipt.ValidateReturnEligibility(nonPurchasedProduct, 1m, pastReturns)
        );
    }

    [Fact]
    public void ValidateReturnEligibility_ReturnQuantityExceedsPurchasedQuantity_ShouldThrowInvalidOperationException()
    {
        var receipt = new Receipt(Guid.NewGuid());
        var productId = Guid.NewGuid();
        
        receipt.AddDomainItem(new ReceiptItem(receipt.Id, productId, Guid.NewGuid(), 3m, 10m));
        var pastReturns = new List<ReturnTransaction>();

        Assert.Throws<InvalidOperationException>(() =>
            receipt.ValidateReturnEligibility(productId, 4m, pastReturns)
        );
    }

    [Fact]
    public void ValidateReturnEligibility_WithPastReturnsExceedingLimit_ShouldThrowInvalidOperationException()
    {
        var receipt = new Receipt(Guid.NewGuid());
        var productId = Guid.NewGuid();
        var batchId = Guid.NewGuid();
        
        receipt.AddDomainItem(new ReceiptItem(receipt.Id, productId, batchId, 5m, 10m));

        var returnTx = new ReturnTransaction(receipt.Id, Guid.NewGuid());
        returnTx.AddItem(productId, batchId, 3m, 10m, true);

        var pastReturns = new List<ReturnTransaction> { returnTx };

        Assert.Throws<InvalidOperationException>(() =>
            receipt.ValidateReturnEligibility(productId, 3m, pastReturns)
        );
    }

    [Fact]
    public void ValidateReturnEligibility_ValidReturnWithPastPartialReturns_ShouldPassSuccessfully()
    {
        var receipt = new Receipt(Guid.NewGuid());
        var productId = Guid.NewGuid();
        var batchId = Guid.NewGuid();
        
        receipt.AddDomainItem(new ReceiptItem(receipt.Id, productId, batchId, 5m, 10m));

        var returnTx = new ReturnTransaction(receipt.Id, Guid.NewGuid());
        returnTx.AddItem(productId, batchId, 2m, 10m, true);

        var pastReturns = new List<ReturnTransaction> { returnTx };

        var exception = Record.Exception(() => 
            receipt.ValidateReturnEligibility(productId, 2m, pastReturns)
        );

        Assert.Null(exception);
    }
}