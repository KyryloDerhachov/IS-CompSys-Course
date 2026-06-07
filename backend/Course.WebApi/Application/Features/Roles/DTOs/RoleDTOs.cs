namespace Course.WebApi.Features.Roles.DTOs;

public record RoleDTO(
    Guid Id,
    string Name, 
    string NameUKR, 
    string Description
    );
