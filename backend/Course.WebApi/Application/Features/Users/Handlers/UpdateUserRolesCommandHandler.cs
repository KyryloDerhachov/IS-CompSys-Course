using Course.WebApi.Application.Interfaces;
using Course.WebApi.Features.Users.Commands;
using MediatR;

namespace Course.WebApi.Features.Users.Handlers;

public class UpdateUserRolesCommandHandler : IRequestHandler<UpdateUserRolesCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public UpdateUserRolesCommandHandler(IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    public async Task Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);

        if (user == null)
            throw new KeyNotFoundException($"The user with ID {request.UserId} was not found");

        user.ClearRoles();

        foreach (var roleId in request.RoleIds)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role != null)
            {
                user.AddRole(role);
            }
        }

        await _userRepository.UpdateAsync(user);
    }
}