using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Course.WebApi.Domain.Entities;
using Course.WebApi.Application.Interfaces;

namespace Course.WebApi.Infrastructure.Persistence;

public class EfSupplyRepository : ISupplyRepository
{
    private readonly ApplicationDbContext _context;

    public EfSupplyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Supply?> GetByIdAsync(Guid id)
    {
        return await _context.Supplies
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Supply>> GetAllAsync()
    {
        return await _context.Supplies
            .Include(s => s.Items)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddAsync(Supply supply)
    {
        await _context.Supplies.AddAsync(supply);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Supply supply)
    {
        supply.IncrementVersion();
        _context.Supplies.Update(supply);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new InvalidOperationException(
                $"Concurrency conflict: The delivery document from supplier '{supply.SupplierName}' was modified by another process.", ex);
        }
    }
}