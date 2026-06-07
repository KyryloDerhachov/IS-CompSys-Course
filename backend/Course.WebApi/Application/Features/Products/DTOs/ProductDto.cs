namespace Course.WebApi.Features.Products.DTOs;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Unit { get; set; } = "шт";
    public decimal? DefaultPurchasePrice { get; set; }
    public decimal? DefaultSalePrice { get; set; }
    public bool IsActive { get; set; }
    public int Version { get; set; }

    public Dictionary<string, string> Attributes { get; set; } = new();
}