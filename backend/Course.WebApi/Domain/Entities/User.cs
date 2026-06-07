using Course.WebApi.Domain.Common;

namespace Course.WebApi.Domain.Entities;

public class User : AuditableEntity<Guid>
{
    public string Login { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;

    public bool IsActive { get; private set; } = true;
    public DateTime? LockoutUntil { get; private set; }
    public int FailedLoginAttempts { get; private set; } = 0;

    private readonly List<Role> _roles = new();
    public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();

    public User() { }

    public User(string login, string email, string firstName, string lastName)
    {
        Id = Guid.NewGuid();
        Login = login.Trim();
        Email = email.Trim().ToLower();
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
    }

    public void SetPassword(string hash) => PasswordHash = hash;
    public void AddRole(Role role) => _roles.Add(role);
    public void ClearRoles() => _roles.Clear();
    public void Update(string email, string firstName, string lastName) {
        Email = email.Trim().ToLower();
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
    }
    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;

    public void RecordFailedLogin()
    {
        FailedLoginAttempts++;
        if (FailedLoginAttempts >= 5)
            LockoutUntil = DateTime.UtcNow.AddMinutes(15);
    }

    public void ResetFailedAttempts()
    {
        FailedLoginAttempts = 0;
        LockoutUntil = null;
    }

    public bool IsLockedOut() => LockoutUntil.HasValue && LockoutUntil > DateTime.UtcNow;
}