using Course.WebApi.Application.Interfaces;
using Course.WebApi.Features.Users.DTOs;
using Course.WebApi.Features.Users.Queries;
using MediatR;

namespace Course.WebApi.Features.Users.Handlers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UserDto>>
{
    private readonly IUserRepository _userRepository;

    public GetUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync(); 

        var query = users.AsQueryable();

        if (!request.IncludeInactive)
        {
            query = query.Where(u => u.IsActive);
        }

        return query.Select(u => new UserDto
        {
            Id = u.Id,
            Login = u.Login,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            IsActive = u.IsActive,
            LockoutUntil = u.LockoutUntil,
            Roles = u.Roles.Select(r => r.Name).ToList()
        }).ToList();
    }
}