using Course.WebApi.Domain.Entities;
using Course.WebApi.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Course.WebApi.Infrastructure.Persistence;

public class EfProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public EfProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Product>> GetAllAsync(bool includeInactive = false)
    {
        var query = _context.Products.AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(p => p.IsActive);
        }

        return await query.ToListAsync();
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        
        product.IncrementVersion();
        
        _context.Products.Update(product);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new InvalidOperationException(
                $"There is a data version conflict for the product '{product.Name}'. The product card was modified in the database by another process.", ex);
        }
    }

    public async Task<Product?> GetBySkuAsync(string sku)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Sku == sku);
    }
}