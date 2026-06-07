using Course.WebApi.Features.Users.DTOs;
using MediatR;

namespace Course.WebApi.Features.Users.Queries;

public record GetUsersQuery(bool IncludeInactive = false) : IRequest<List<UserDto>>;