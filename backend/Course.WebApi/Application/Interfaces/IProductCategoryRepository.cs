using Course.WebApi.Domain.Entities;

namespace Course.WebApi.Application.Interfaces;


public interface IProductCategoryRepository
{
    Task<ProductCategory?> GetByIdAsync(Guid id);
    Task<IEnumerable<ProductCategory>> GetAllAsync(bool includeInactive = false);
    Task AddAsync(ProductCategory category);
    Task UpdateAsync(ProductCategory category);
}