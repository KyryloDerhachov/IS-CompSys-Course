namespace Course.WebApi.Features.Supplies.DTOs;
public record SupplyItemDto(
    Guid Id, 
    Guid ProductId, 
    decimal Quantity, 
    decimal PurchasePrice, 
    int ShelfLifeDays);
public record SupplyDto(
    Guid Id, 
    string SupplierName, 
    DateTime SupplyDate, 
    string Status, 
    int Version, 
    List<SupplyItemDto> Items);