namespace Course.WebApi.Features.Batches.DTOs;

public class BatchDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSku { get; set; } = string.Empty;
    public decimal QuantityReceived { get; set; }
    public decimal RemainingQuantity { get; set; }
    public decimal PurchasePrice { get; set; }
    public DateTime ExpirationDate { get; set; }
    public DateTime SupplyDate {get; set;}
    public bool IsExpired => ExpirationDate < DateTime.UtcNow;
    public int Version { get; set; }
}