using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Course.WebApi.Domain.Entities;
using Course.WebApi.Application.Interfaces;

namespace Course.WebApi.Infrastructure.Persistence;

public class EfReturnRepository : IReturnRepository
{
    private readonly ApplicationDbContext _context;

    public EfReturnRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ReturnTransaction>> GetByReceiptIdAsync(Guid receiptId)
    {
        return await _context.ReturnTransactions
            .Include(r => r.Items)
            .Where(r => r.ReceiptId == receiptId)
            .ToListAsync();
    }

    public async Task AddAsync(ReturnTransaction returnTx)
    {
        await _context.ReturnTransactions.AddAsync(returnTx);
        await _context.SaveChangesAsync();
    }
}