using Course.WebApi.Domain.Entities;
using Course.WebApi.Application.Interfaces;
using Course.WebApi.Features.Users.Commands;
using Course.WebApi.Infrastructure.Services.Authentication;
using MediatR;

namespace Course.WebApi.Features.Users.Handlers;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPasswordHasher _passwordHasher;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (!await _userRepository.IsLoginUniqueAsync(request.Login))
            throw new InvalidOperationException("That username is already taken");

        if (!await _userRepository.IsEmailUniqueAsync(request.Email))
            throw new InvalidOperationException("This email address is already taken");

        var user = new User(request.Login, request.Email, request.FirstName, request.LastName);
        user.SetPassword(_passwordHasher.Hash(request.Password));

        foreach (var roleId in request.RoleIds)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role != null) user.AddRole(role);
        }

        await _userRepository.AddAsync(user);
        return user.Id;
    }
}