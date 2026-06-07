using MediatR;

namespace Course.WebApi.Features.Users.Commands;

public record CreateUserCommand(
    string Login,
    string Email,
    string FirstName,
    string LastName,
    string Password,
    List<Guid> RoleIds) : IRequest<Guid>;