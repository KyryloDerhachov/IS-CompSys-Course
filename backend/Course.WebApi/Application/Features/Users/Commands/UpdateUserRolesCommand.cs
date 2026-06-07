using MediatR;

namespace Course.WebApi.Features.Users.Commands;

public record UpdateUserRolesCommand(Guid UserId, List<Guid> RoleIds) : IRequest;