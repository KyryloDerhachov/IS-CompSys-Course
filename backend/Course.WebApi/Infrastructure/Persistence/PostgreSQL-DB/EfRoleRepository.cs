using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Course.WebApi.Domain.Entities;
using Course.WebApi.Application.Interfaces;

namespace Course.WebApi.Infrastructure.Persistence;

public class EfRoleRepository : IRoleRepository
{
    private readonly ApplicationDbContext _context;

    public EfRoleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Role>> GetAllAsync()
    {
        return await _context.Roles.AsNoTracking()
            .ToListAsync();
    }
    public async Task<Role?> GetByIdAsync(Guid id)
    {
        return await _context.Set<Role>()
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        var normalizedName = name.Trim().ToLower();

        return await _context.Set<Role>()
            .FirstOrDefaultAsync(r => r.Name.ToLower() == normalizedName);
    }

    public async Task AddAsync(Role role)
    {
        await _context.Set<Role>().AddAsync(role);
        await _context.SaveChangesAsync();
    }

}