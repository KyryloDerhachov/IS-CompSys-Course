using Course.WebApi.Features.Roles.DTOs;
using MediatR;

namespace Course.WebApi.Features.Roles.Queries;

public record GetAllRolesQuery() : IRequest<List<RoleDTO>>;