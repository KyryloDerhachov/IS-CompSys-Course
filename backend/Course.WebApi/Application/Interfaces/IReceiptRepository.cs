using Course.WebApi.Domain.Entities;

namespace Course.WebApi.Application.Interfaces;

public interface IReceiptRepository
{
    Task AddAsync(Receipt receipt);
    Task<Receipt?> GetByIdAsync(Guid id);
    Task<Receipt?> GetByNumberAsync(string receiptNumber);
    Task<IEnumerable<Receipt>> GetAllAsync();
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}