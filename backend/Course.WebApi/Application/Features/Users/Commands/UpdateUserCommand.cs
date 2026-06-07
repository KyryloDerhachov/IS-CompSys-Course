using MediatR;

namespace Course.WebApi.Features.Users.Commands;

public record UpdateUserCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string Email
) : IRequest;