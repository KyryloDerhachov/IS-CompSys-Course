using MediatR;

namespace Course.WebApi.Features.Users.Commands;

public record DeactivateUserCommand(Guid Id) : IRequest;