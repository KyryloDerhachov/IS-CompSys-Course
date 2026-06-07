using MediatR;
using Course.WebApi.Features.Users.Commands;
using Course.WebApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Course.WebApi.Features.Users.Commands;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
{
    private readonly ApplicationDbContext _context; 

    public UpdateUserCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == command.Id, cancellationToken);

        if (user == null)
            throw new KeyNotFoundException($"User with id {command.Id} not found.");

        user.Update(command.Email, command.FirstName, command.LastName);


        await _context.SaveChangesAsync(cancellationToken);
    }
}