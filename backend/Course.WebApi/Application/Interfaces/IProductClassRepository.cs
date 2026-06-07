using Course.WebApi.Domain.Entities;

namespace Course.WebApi.Application.Interfaces;


public interface IProductClassRepository
{
    Task<ProductClass?> GetByIdAsync(Guid id);
    Task<IEnumerable<ProductClass>> GetAllAsync(bool includeInactive = false);
    Task AddAsync(ProductClass productClass);
    Task UpdateAsync(ProductClass productClass);
}