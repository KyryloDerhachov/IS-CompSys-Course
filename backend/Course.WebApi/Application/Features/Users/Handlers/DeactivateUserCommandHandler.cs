using Course.WebApi.Application.Interfaces;
using Course.WebApi.Features.Users.Commands;
using MediatR;

namespace Course.WebApi.Features.Users.Handlers;

public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand>
{
    private readonly IUserRepository _userRepository;

    public DeactivateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id);

        if (user == null)
            throw new KeyNotFoundException($"The user with ID {request.Id} was not found");
        user.Deactivate();

        await _userRepository.UpdateAsync(user);
    }
}