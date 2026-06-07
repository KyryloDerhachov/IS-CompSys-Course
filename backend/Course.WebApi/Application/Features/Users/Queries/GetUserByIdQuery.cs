using Course.WebApi.Features.Users.DTOs;
using MediatR;

namespace Course.WebApi.Features.Users.Queries;

public record GetUserByIdQuery(Guid Id) : IRequest<UserDto>;