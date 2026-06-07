namespace Course.WebApi.Features.Sales.DTOs;

public record ReceiptItemDto(
    Guid ProductId, 
    string ProductName, 
    decimal Quantity, 
    decimal Price, 
    decimal Total
);
public record ReceiptDto(
    Guid Id, 
    string ReceiptNumber, 
    DateTime SaleDate, 
    decimal TotalAmount, 
    List<ReceiptItemDto> Items
);

public record SaleItemInput(
    Guid ProductId, 
    decimal Quantity, 
    decimal Price);

public record ReturnItemInput(
    Guid ProductId, 
    decimal Quantity);

public record ReturnTransactionDto(
    Guid Id, 
    Guid ReceiptId, 
    DateTime CreatedAt, 
    decimal TotalAmount, 
    List<ReturnItemDto> Items);
public record ReturnItemDto(
    Guid ProductId, 
    Guid BatchId, 
    decimal Quantity, 
    decimal Price, 
    bool ReturnedToStock);