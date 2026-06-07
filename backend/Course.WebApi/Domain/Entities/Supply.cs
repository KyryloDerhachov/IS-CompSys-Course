using Course.WebApi.Domain.Common;

namespace Course.WebApi.Domain.Entities;

public enum SupplyStatus { Draft, Posted }

public class Supply : AuditableEntity<Guid>
{
    public string SupplierName { get; private set; } = string.Empty;
    public DateTime SupplyDate { get; private set; }
    public SupplyStatus Status { get; private set; } = SupplyStatus.Draft;

    private readonly List<SupplyItem> _items = new();
    public IReadOnlyCollection<SupplyItem> Items => _items.AsReadOnly();

    public Supply() { }

    public Supply(string supplierName, DateTime supplyDate, Guid? userId)
    {
        if (string.IsNullOrWhiteSpace(supplierName))
            throw new ArgumentException("The supplier name cannot be left blank.");

        Id = Guid.NewGuid();
        SupplierName = supplierName.Trim();
        SupplyDate = supplyDate;
        Status = SupplyStatus.Draft;
        SetCreated(userId);
    }

    public void AddItem(Product product, decimal quantity, decimal purchasePrice, int shelfLifeDays)
    {
        if (Status == SupplyStatus.Posted)
            throw new InvalidOperationException("You cannot edit a received shipment.");

        if (quantity <= 0 || purchasePrice <= 0 || shelfLifeDays <= 0)
            throw new ArgumentException("Product parameters must be strictly positive numbers.");

        if (purchasePrice > product.DefaultSalePrice)
            throw new InvalidOperationException($"The purchase price ({purchasePrice}) is higher than the default retail price ({product.DefaultSalePrice}) for '{product.Name}'.");

        _items.Add(new SupplyItem(Id, product.Id, quantity, purchasePrice, shelfLifeDays));
    }

    public void Post(Guid? userId)
    {
        if (Status == SupplyStatus.Posted)
            throw new InvalidOperationException("The shipment has already been received.");

        Status = SupplyStatus.Posted;
        SetUpdated(userId);
    }
}

public class SupplyItem
{
    public Guid Id { get; private set; }
    public Guid SupplyId { get; private set; }
    public Guid ProductId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal PurchasePrice { get; private set; }
    public int ShelfLifeDays { get; private set; } 

    public SupplyItem(Guid supplyId, Guid productId, decimal quantity, decimal purchasePrice, int shelfLifeDays)
    {
        Id = Guid.NewGuid();
        SupplyId = supplyId;
        ProductId = productId;
        Quantity = quantity;
        PurchasePrice = purchasePrice;
        ShelfLifeDays = shelfLifeDays;
    }
}