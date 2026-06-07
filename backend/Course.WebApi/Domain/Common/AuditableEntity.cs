namespace Course.WebApi.Domain.Common;

public abstract class AuditableEntity<TId> : BaseEntity<TId>
{
    public Guid? CreatedBy { get; protected set; }
    public Guid? UpdatedBy { get; protected set; }
    public int Version { get; private set; } = 1;

    public void SetCreated(Guid? userId)
    {
        CreatedAt = DateTime.UtcNow;
        CreatedBy = userId;
        Version = 1; 
    }

    public void SetUpdated(Guid? userId)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = userId;

    }

    public void IncrementVersion()
    {
        Version++;
    }
}