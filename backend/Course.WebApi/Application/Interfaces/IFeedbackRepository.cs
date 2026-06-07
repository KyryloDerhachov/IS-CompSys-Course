using Course.WebApi.Domain.Entities;

namespace Course.WebApi.Application.Interfaces;

public interface IFeedbackRepository
{
    Task<FeedbackRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<FeedbackRecord?> GetByReceiptNumberAsync(string receiptNumber, CancellationToken cancellationToken);
    Task<IEnumerable<FeedbackRecord>> GetAllAsync(CancellationToken cancellationToken);
    Task AddAsync(FeedbackRecord record, CancellationToken cancellationToken);
    Task UpdateAsync(FeedbackRecord record);
}