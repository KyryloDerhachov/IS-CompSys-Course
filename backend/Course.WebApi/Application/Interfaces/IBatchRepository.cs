using Course.WebApi.Domain.Entities;

namespace Course.WebApi.Application.Interfaces;

public interface IBatchRepository
{
    Task AddAsync(Batch batch);
    Task UpdateAsync(Batch batch);
    Task<Batch?> GetByIdAsync(Guid id);
    Task<IEnumerable<Batch>> GetAllAsync();
    Task<IEnumerable<Batch>> GetActiveBatchesByProductAsync(Guid productId);
}