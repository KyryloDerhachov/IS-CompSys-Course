using Course.WebApi.Domain.Common;

namespace Course.WebApi.Domain.Entities;

public class ProductCategory : AuditableEntity<Guid>
{
    public Guid? ParentId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true; 

    public ProductCategory() { }

    public ProductCategory(Guid? parentId, string code, string name, string? description, Guid? userId)
    {
        Id = Guid.NewGuid();
        ParentId = parentId;
        Code = code.Trim().ToLower();
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

    public void Deactivate(Guid? Id)
    {
        IsActive = false;
        SetUpdated(Id);
    }
    public void Activate(Guid? Id)
    {
        IsActive = true;
        SetUpdated(Id);
    }
}