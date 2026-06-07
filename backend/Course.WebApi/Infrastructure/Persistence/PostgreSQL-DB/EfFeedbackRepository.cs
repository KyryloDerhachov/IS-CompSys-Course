using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Course.WebApi.Application.Interfaces;
using Course.WebApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Course.WebApi.Infrastructure.Persistence;

public class FeedbackRepository : IFeedbackRepository
{
    private readonly ApplicationDbContext _context;

    public FeedbackRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<FeedbackRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Set<FeedbackRecord>()
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
    }

    public async Task<FeedbackRecord?> GetByReceiptNumberAsync(string receiptNumber, CancellationToken cancellationToken)
    {
        var normalizedNumber = receiptNumber.Trim().ToUpper();
        return await _context.Set<FeedbackRecord>()
            .FirstOrDefaultAsync(f => f.ReceiptNumber == normalizedNumber, cancellationToken);
    }

    public async Task<IEnumerable<FeedbackRecord>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Set<FeedbackRecord>()
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(FeedbackRecord record, CancellationToken cancellationToken)
    {
        await _context.Set<FeedbackRecord>().AddAsync(record, cancellationToken);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(FeedbackRecord record)
    {
        _context.Set<FeedbackRecord>().Update(record);
        await _context.SaveChangesAsync();
    }
}