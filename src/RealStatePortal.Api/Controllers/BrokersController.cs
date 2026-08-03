using Microsoft.AspNetCore.Mvc;
using RealStatePortal.Application.Brokers;

namespace RealStatePortal.Api.Controllers;

[ApiController]
[Route("api/brokers")]
public sealed class BrokersController(IBrokerAdministrationService brokerService) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyCollection<BrokerDto>> GetAll(CancellationToken cancellationToken) =>
        brokerService.GetAllAsync(cancellationToken);

    [HttpGet("{userId:guid}")]
    public Task<BrokerDto> GetByUserId(Guid userId, CancellationToken cancellationToken) =>
        brokerService.GetByUserIdAsync(userId, cancellationToken);

    [HttpPost]
    public async Task<ActionResult<BrokerDto>> Create(CreateBrokerRequest request, CancellationToken cancellationToken)
    {
        var broker = await brokerService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetByUserId), new { userId = broker.UserId }, broker);
    }

    [HttpPost("{userId:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid userId, CancellationToken cancellationToken)
    {
        await brokerService.DeactivateAsync(userId, cancellationToken);
        return NoContent();
    }
}