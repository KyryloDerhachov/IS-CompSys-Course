using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Course.WebApi.Domain.Entities;
using Course.WebApi.Application.Interfaces;

namespace Course.WebApi.Infrastructure.Persistence;

public class EfUserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public EfUserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        var normalizedLogin = username.Trim().ToLower();
        
        return await _context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Login.ToLower() == normalizedLogin);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users
            .Include(u => u.Roles)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        user.IncrementVersion();
        _context.Users.Update(user);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new InvalidOperationException(
                $"Concurrency conflict: The user data '{user.Login}' has been modified in the database by another process.", ex);
        }
    }
    public async Task<User?> GetByLoginAsync(string login)
    {
        var normalizedLogin = login.Trim().ToLower();
        
        return await _context.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Login.ToLower() == normalizedLogin);
    }

    public async Task<bool> IsLoginUniqueAsync(string login, Guid? excludeId = null)
    {
        var normalizedLogin = login.Trim().ToLower();
        return !await _context.Users
            .AnyAsync(u => u.Login.ToLower() == normalizedLogin && u.Id != excludeId);
    }

    public async Task<bool> IsEmailUniqueAsync(string email, Guid? excludeId = null)
    {
        var normalizedEmail = email.Trim().ToLower();

        return !await _context.Users
            .AnyAsync(u => u.Email.ToLower() == normalizedEmail && u.Id != excludeId);
    }

    public async Task DeleteAsync(User user)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }
}