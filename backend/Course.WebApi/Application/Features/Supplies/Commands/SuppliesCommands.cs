using MediatR;

namespace Course.WebApi.Features.Supplies.Commands;


public record CreateSupplyItemInput(
    Guid ProductId, 
    decimal Quantity, 
    decimal PurchasePrice, 
    int ShelfLifeDays);
public record CreateSupplyCommand(
    string SupplierName, 
    DateTime SupplyDate, 
    List<CreateSupplyItemInput> Items, 
    Guid? CurrentUserId) : IRequest<Guid>;

public record PostSupplyCommand(
    Guid SupplyId, 
    int Version, 
    Guid? CurrentUserId) : IRequest;