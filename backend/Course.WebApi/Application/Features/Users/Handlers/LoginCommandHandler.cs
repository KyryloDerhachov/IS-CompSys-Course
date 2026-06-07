using Course.WebApi.Application.Interfaces;
using Course.WebApi.Features.Users.Commands;
using Course.WebApi.Features.Users.DTOs;
using Course.WebApi.Infrastructure.Services.Authentication;
using MediatR;

namespace Course.WebApi.Features.Users.Handlers;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRoleRepository _roleRepository;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _roleRepository = roleRepository;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByLoginAsync(request.Login);

        if (user == null || !user.IsActive || user.IsLockedOut())
            throw new UnauthorizedAccessException("Incorrect username or password");

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            user.RecordFailedLogin();
            throw new UnauthorizedAccessException("Incorrect username or password");
        }

        user.ResetFailedAttempts();

        var roles = user.Roles.Select(r => r.Name).ToList();
        var token = _jwtTokenService.GenerateToken(user.Id, user.Login, roles);

        return new AuthResponse
        {
            UserId = user.Id,
            Login = user.Login,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Token = token,
            Roles = roles
        };
    }
}