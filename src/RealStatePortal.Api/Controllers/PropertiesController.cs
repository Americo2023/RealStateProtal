using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealStatePortal.Application.Properties.Dtos;
using RealStatePortal.Application.Properties.Services;

namespace RealStatePortal.Api.Controllers;

[Route("api/properties")]
public sealed class PropertiesController(IPropertyService propertyService) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetPublished(
        [FromQuery] string? query,
        [FromQuery] PropertySearchRequest request,
        CancellationToken cancellationToken)
    {
        var searchRequest = request with { Query = query ?? request.Query };
        var result = string.IsNullOrWhiteSpace(query) && request with { } == new PropertySearchRequest(null, null, null, null, null, null, null, null, null, null)
            ? await propertyService.GetPublishedAsync(cancellationToken)
            : await propertyService.SearchAsync(searchRequest, cancellationToken);

        return FromResult(result);
    }

    [HttpGet("mine")]
    [Authorize(Policy = "BrokerOrAdministrator")]
    public async Task<IActionResult> GetMine(CancellationToken cancellationToken)
    {
        var result = await propertyService.GetMineAsync(cancellationToken);
        return FromResult(result);
    }

    [HttpGet("all")]
    [Authorize(Policy = "Administrator")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        FromResult(await propertyService.GetAllAsync(cancellationToken));

    [HttpGet("{propertyId:guid}")]
    public async Task<IActionResult> GetById(Guid propertyId, CancellationToken cancellationToken)
    {
        var result = await propertyService.GetByIdAsync(propertyId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new ProblemDetails { Detail = result.Error });
    }

    [HttpPost]
    [Authorize(Policy = "BrokerOrAdministrator")]
    public async Task<IActionResult> Create(
        [FromBody] CreatePropertyRequest request,
        CancellationToken cancellationToken)
    {
        var result = await propertyService.CreateAsync(request, cancellationToken);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { propertyId = result.Value!.Id }, result.Value)
            : FromResult(result);
    }

    [HttpPut("{propertyId:guid}")]
    [Authorize(Policy = "BrokerOrAdministrator")]
    public async Task<IActionResult> Update(
        Guid propertyId,
        [FromBody] UpdatePropertyRequest request,
        CancellationToken cancellationToken)
    {
        var result = await propertyService.UpdateAsync(propertyId, request, cancellationToken);
        return FromResult(result);
    }

    [HttpPost("{propertyId:guid}/publish")]
    [Authorize(Policy = "BrokerOrAdministrator")]
    public Task<IActionResult> Publish(Guid propertyId, CancellationToken cancellationToken) => ChangeStatus(
        () => propertyService.PublishAsync(propertyId, cancellationToken));

    [HttpPost("{propertyId:guid}/withdraw")]
    [Authorize(Policy = "BrokerOrAdministrator")]
    public Task<IActionResult> Withdraw(Guid propertyId, CancellationToken cancellationToken) => ChangeStatus(
        () => propertyService.WithdrawAsync(propertyId, cancellationToken));

    [HttpPost("{propertyId:guid}/sold")]
    [Authorize(Policy = "BrokerOrAdministrator")]
    public Task<IActionResult> MarkAsSold(Guid propertyId, CancellationToken cancellationToken) => ChangeStatus(
        () => propertyService.MarkAsSoldAsync(propertyId, cancellationToken));

    [HttpDelete("{propertyId:guid}")]
    [Authorize(Policy = "BrokerOrAdministrator")]
    public Task<IActionResult> Delete(Guid propertyId, CancellationToken cancellationToken) => ChangeStatus(
        () => propertyService.DeleteAsync(propertyId, cancellationToken));

    [HttpPost("{propertyId:guid}/images")]
    [Authorize(Policy = "BrokerOrAdministrator")]
    public async Task<IActionResult> AddImage(Guid propertyId, IFormFile file, [FromForm] string altText, [FromForm] bool isPrimary, CancellationToken cancellationToken)
    {
        if (file.Length == 0)
        {
            return BadRequest(new ProblemDetails { Detail = "An image file is required." });
        }

        await using var content = file.OpenReadStream();
        var result = await propertyService.AddImageAsync(propertyId, content, file.FileName, altText, isPrimary, cancellationToken);
        return FromResult(result);
    }

    [HttpDelete("{propertyId:guid}/images/{imageId:guid}")]
    [Authorize(Policy = "BrokerOrAdministrator")]
    public async Task<IActionResult> RemoveImage(Guid propertyId, Guid imageId, CancellationToken cancellationToken) =>
        FromResult(await propertyService.RemoveImageAsync(propertyId, imageId, cancellationToken));

    [HttpPost("{propertyId:guid}/images/{imageId:guid}/primary")]
    [Authorize(Policy = "BrokerOrAdministrator")]
    public async Task<IActionResult> SetPrimaryImage(Guid propertyId, Guid imageId, CancellationToken cancellationToken) =>
        FromResult(await propertyService.SetPrimaryImageAsync(propertyId, imageId, cancellationToken));

    private async Task<IActionResult> ChangeStatus(Func<Task<RealStatePortal.Application.Common.Result>> operation)
    {
        return FromResult(await operation());
    }
}