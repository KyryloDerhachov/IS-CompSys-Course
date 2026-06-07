using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Course.WebApi.Domain.Entities;
using Course.WebApi.Application.Interfaces;

namespace Course.WebApi.Infrastructure.Persistence;

public class EfReceiptRepository : IReceiptRepository
{
    private readonly ApplicationDbContext _context;

    public EfReceiptRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Receipt?> GetByIdAsync(Guid id)
    {
        return await _context.Receipts
            .Include(r => r.Items)
            .FirstOrDefaultAsync(r => r.Id == id);
    }
    public async Task<Receipt?>  GetByNumberAsync(string receiptNumber)
    {
        return await _context.Receipts
            .Include(r => r.Items)
            .FirstOrDefaultAsync(r => r.ReceiptNumber == receiptNumber);
    }

    public async Task<IEnumerable<Receipt>> GetAllAsync()
    {
        return await _context.Receipts
            .Include(r => r.Items)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddAsync(Receipt receipt)
    {
        await _context.Receipts.AddAsync(receipt);
        await _context.SaveChangesAsync();
    }
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}