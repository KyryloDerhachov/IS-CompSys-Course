using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Course.WebApi.Domain.Entities;
using Course.WebApi.Application.Interfaces;

namespace Course.WebApi.Infrastructure.Persistence;

public class EfProductClassRepository : IProductClassRepository
{
    private readonly ApplicationDbContext _context;

    public EfProductClassRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductClass?> GetByIdAsync(Guid id)
    {
        return await _context.Set<ProductClass>()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<ProductClass>> GetAllAsync(bool includeInactive = false)
    {
        var query = _context.Set<ProductClass>().AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(c => c.IsActive);
        }

        return await query.AsNoTracking().ToListAsync();
    }

    public async Task AddAsync(ProductClass productClass)
    {
        await _context.Set<ProductClass>().AddAsync(productClass);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ProductClass productClass)
    {
        productClass.IncrementVersion();
        _context.Set<ProductClass>().Update(productClass);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new InvalidOperationException(
                $"Concurrency conflict: The product class '{productClass.Name}' was modified by another process.", ex);
        }
    }
}