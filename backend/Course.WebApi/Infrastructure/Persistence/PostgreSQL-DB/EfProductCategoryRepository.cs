using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Course.WebApi.Domain.Entities;
using Course.WebApi.Application.Interfaces;

namespace Course.WebApi.Infrastructure.Persistence;

public class EfProductCategoryRepository : IProductCategoryRepository
{
    private readonly ApplicationDbContext _context;

    public EfProductCategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductCategory?> GetByIdAsync(Guid id)
    {
        return await _context.Set<ProductCategory>()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<ProductCategory>> GetAllAsync(bool includeInactive = false)
    {
        var query = _context.Set<ProductCategory>().AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(c => c.IsActive);
        }

        return await query.AsNoTracking().ToListAsync();
    }

    public async Task AddAsync(ProductCategory category)
    {
        await _context.Set<ProductCategory>().AddAsync(category);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ProductCategory category)
    {
        category.IncrementVersion();
        _context.Set<ProductCategory>().Update(category);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new InvalidOperationException(
                $"Parallelism conflict: The product category '{category.Name}' was modified by another process.", ex);
        }
    }
}