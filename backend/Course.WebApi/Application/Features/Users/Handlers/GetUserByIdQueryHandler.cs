using Course.WebApi.Application.Interfaces;
using Course.WebApi.Features.Users.DTOs;
using Course.WebApi.Features.Users.Queries;
using MediatR;

namespace Course.WebApi.Features.Users.Handlers;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id);
        
        if (user == null)
            throw new KeyNotFoundException($"The user with ID {request.Id} was not found");

        return new UserDto
        {
            Id = user.Id,
            Login = user.Login,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsActive = user.IsActive,
            LockoutUntil = user.LockoutUntil,
            Roles = user.Roles.Select(r => r.Name).ToList()
        };
    }
}