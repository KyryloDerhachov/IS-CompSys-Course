namespace Course.WebApi.Features.Products.DTOs;

public class UpdateProductDTO
{
    public string Name { get; set; } = string.Empty;
    public Guid ClassId { get; set; }
    public Guid CategoryId { get; set; }
    public string Unit { get; set; } = "шт";
    public int Version { get; set; } 
    public Dictionary<string, string>? Attributes { get; set; } 
}