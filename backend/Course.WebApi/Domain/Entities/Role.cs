using Course.WebApi.Domain.Common;

namespace Course.WebApi.Domain.Entities;

public class Role : AuditableEntity<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public string NameUKR { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    public Role() { }
    public Role(string name, string nameUkr, string description)
    {
        Name = name;
        NameUKR = nameUkr;
        Description = description;
    }
}