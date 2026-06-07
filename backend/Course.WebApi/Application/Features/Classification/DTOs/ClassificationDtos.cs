namespace Course.WebApi.Features.Classification.DTOs;

public record ProductClassDto(Guid Id, string Code, string Name, string? Description, bool IsActive, int Version);

public record ProductCategoryDto(Guid Id, Guid? ParentId, string Code, string Name, string? Description, bool IsActive, int Version);