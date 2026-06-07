using Course.WebApi.Domain.Entities;

namespace Course.WebApi.Application.Interfaces;

public interface ISupplyRepository
{
    Task<Supply?> GetByIdAsync(Guid id);
    Task<IEnumerable<Supply>> GetAllAsync();
    Task AddAsync(Supply supply);
    Task UpdateAsync(Supply supply);
}

