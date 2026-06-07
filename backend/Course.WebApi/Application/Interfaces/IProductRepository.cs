using Course.WebApi.Domain.Entities;

namespace Course.WebApi.Application.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id);
    Task<Product?> GetBySkuAsync(string sku);
    Task<IEnumerable<Product>> GetAllAsync(bool includeInactive = false);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
}