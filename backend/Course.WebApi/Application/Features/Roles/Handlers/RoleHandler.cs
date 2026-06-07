using Course.WebApi.Application.Interfaces;
using Course.WebApi.Features.Roles.DTOs;
using Course.WebApi.Features.Roles.Queries;
using MediatR;

namespace Course.WebApi.Features.Roles.Handlers;

public class RolesHandlers : IRequestHandler<GetAllRolesQuery, List<RoleDTO>>

{
    private readonly IRoleRepository _roleRepository;
     public RolesHandlers(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }
    public async Task<List<RoleDTO>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await _roleRepository.GetAllAsync();

        return roles.Select(r => new RoleDTO(
            r.Id,
            r.Name,
            r.NameUKR,
            r.Description
        )).ToList();
    }
}