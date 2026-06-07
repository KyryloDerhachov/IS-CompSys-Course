using Course.WebApi.Features.Roles.DTOs;
using Course.WebApi.Features.Roles.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Course.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly IMediator _mediator;
    public RolesController(IMediator mediator) => _mediator = mediator;


    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<RoleDTO>))]
    public async Task<ActionResult<List<RoleDTO>>> GetRoles()
        => Ok(await _mediator.Send(new GetAllRolesQuery()));

    
}

