using Course.WebApi.Domain.Entities;

namespace Course.WebApi.Application.Interfaces;

public interface IReturnRepository
{
    Task AddAsync(ReturnTransaction returnTx);
    Task<IEnumerable<ReturnTransaction>> GetByReceiptIdAsync(Guid receiptId);
}