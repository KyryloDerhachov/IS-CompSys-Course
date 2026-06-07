using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Course.WebApi.Domain.Entities;
using Course.WebApi.Application.Interfaces;

namespace Course.WebApi.Infrastructure.Persistence;

public class EfBatchRepository : IBatchRepository
{
    private readonly ApplicationDbContext _context;

    public EfBatchRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Batch?> GetByIdAsync(Guid id)
    {
        return await _context.Batches
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<Batch>> GetAllAsync()
    {
        return await _context.Batches
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Batch>> GetActiveBatchesByProductAsync(Guid productId)
    {
        
        return await _context.Batches
            .Where(b => b.ProductId == productId 
                     && b.RemainingQuantity > 0 
                     && b.ExpirationDate > DateTime.UtcNow)
            .OrderBy(b => b.ExpirationDate) 
            .ToListAsync();
    }

    public async Task AddAsync(Batch batch)
    {
        await _context.Batches.AddAsync(batch);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Batch batch)
    {
        batch.IncrementVersion();
        _context.Batches.Update(batch);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new InvalidOperationException(
                $"Concurrency conflict: The warehouse batch with ID '{batch.Id}' was modified by another process.", ex);
        }
    }
}