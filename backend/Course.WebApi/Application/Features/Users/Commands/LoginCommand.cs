using Course.WebApi.Features.Users.DTOs;
using MediatR;

namespace Course.WebApi.Features.Users.Commands;

public record LoginCommand(string Login, string Password) : IRequest<AuthResponse>;