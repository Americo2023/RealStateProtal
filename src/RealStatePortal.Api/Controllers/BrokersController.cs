using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealStatePortal.Application.Brokers;

namespace RealStatePortal.Api.Controllers;

[Route("api/brokers")]
[Authorize(Policy = "Administrator")]
public sealed class BrokersController(IBrokerService brokerService) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        FromResult(await brokerService.GetAllAsync(cancellationToken));

    [HttpPut("{brokerId:guid}")]
    public async Task<IActionResult> Update(Guid brokerId, [FromBody] UpdateBrokerRequest request, CancellationToken cancellationToken) =>
        FromResult(await brokerService.UpdateAsync(brokerId, request, cancellationToken));
}