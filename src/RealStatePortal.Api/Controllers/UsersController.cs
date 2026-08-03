using Microsoft.AspNetCore.Mvc;
using RealStatePortal.Application.Users;
using RealStatePortal.Domain.Enums;

namespace RealStatePortal.Api.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController(IUserAdministrationService userService) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyCollection<UserDto>> GetAll(CancellationToken cancellationToken) =>
        userService.GetAllAsync(cancellationToken);

    [HttpGet("{id:guid}")]
    public Task<UserDto> GetById(Guid id, CancellationToken cancellationToken) =>
        userService.GetByIdAsync(id, cancellationToken);

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await userService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPut("{id:guid}/role")]
    public async Task<IActionResult> ChangeRole(Guid id, [FromBody] UserRole role, CancellationToken cancellationToken)
    {
        await userService.ChangeRoleAsync(id, role, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        await userService.DeactivateAsync(id, cancellationToken);
        return NoContent();
    }
}