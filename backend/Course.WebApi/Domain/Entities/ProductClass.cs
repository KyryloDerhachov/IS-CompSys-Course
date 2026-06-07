using Course.WebApi.Domain.Common;

namespace Course.WebApi.Domain.Entities;

public class ProductClass : AuditableEntity<Guid>
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true; 

    public ProductClass() { }

    public ProductClass(string code, string name, string? description, Guid? userId)
    {
        Id = Guid.NewGuid();
        Code = code.Trim().ToUpper();
        Name = name.Trim();
        Description = description?.Trim();
        IsActive = true;
        SetCreated(userId);
    }

   
    public void UpdateDescription(string? description, Guid? userId)
    {
        Description = description?.Trim();
        SetUpdated(userId);
    }

    public void Deactivate(Guid? userId)
    {
        IsActive = false;
        SetUpdated(userId);
    }
    public void Activate(Guid? userId)
    {
        IsActive = true;
        SetUpdated(userId);
    }
}