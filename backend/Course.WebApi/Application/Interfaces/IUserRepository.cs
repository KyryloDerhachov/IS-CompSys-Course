using Course.WebApi.Domain.Entities;

namespace Course.WebApi.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByLoginAsync(string login);
    Task<bool> IsLoginUniqueAsync(string login, Guid? excludeId = null);
    Task<bool> IsEmailUniqueAsync(string email, Guid? excludeId = null);
    Task<IEnumerable<User>> GetAllAsync();
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);
}