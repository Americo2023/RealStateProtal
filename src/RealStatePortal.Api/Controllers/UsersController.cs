using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealStatePortal.Application.Users;
using RealStatePortal.Api.Authorization;

namespace RealStatePortal.Api.Controllers;

[Route("api/users")]
[Authorize(Policy = "Administrator")]
public sealed class UsersController(IUserService userService) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        FromResult(await userService.GetAllAsync(cancellationToken));

    [HttpPut("{userId:guid}")]
    public async Task<IActionResult> Update(
        Guid userId,
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken) =>
        FromResult(await userService.UpdateAsync(userId, request, cancellationToken));
}