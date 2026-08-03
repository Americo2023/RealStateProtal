using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RealStatePortal.Application.Properties;

namespace RealStatePortal.Api.Controllers;

[ApiController]
[Route("api/properties")]
public sealed class PropertiesController(IPropertyService propertyService) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyCollection<PropertyDto>> Search([FromQuery] PropertySearchCriteria criteria, CancellationToken cancellationToken) =>
        propertyService.SearchAsync(criteria, cancellationToken);

    [HttpGet("{id:guid}")]
    public Task<PropertyDto> GetById(Guid id, CancellationToken cancellationToken) =>
        propertyService.GetByIdAsync(id, cancellationToken);

    [HttpPost]
    [Authorize(Policy = "Broker")]
    public async Task<ActionResult<PropertyDto>> Create(CreatePropertyRequest request, CancellationToken cancellationToken)
    {
        var property = await propertyService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = property.Id }, property);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Broker")]
    public async Task<IActionResult> Update(Guid id, UpdatePropertyRequest request, CancellationToken cancellationToken)
    {
        await propertyService.UpdateAsync(id, request, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/address")]
    [Authorize(Policy = "Broker")]
    public async Task<IActionResult> SetAddress(Guid id, SetPropertyAddressRequest request, CancellationToken cancellationToken)
    {
        await propertyService.SetAddressAsync(id, request, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/transfer/{brokerId:guid}")]
    [Authorize(Policy = "Broker")]
    public async Task<IActionResult> Transfer(Guid id, Guid brokerId, CancellationToken cancellationToken)
    {
        await propertyService.TransferAsync(id, brokerId, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/publish")]
    [Authorize(Policy = "Broker")]
    public async Task<IActionResult> Publish(Guid id, CancellationToken cancellationToken)
    {
        await propertyService.PublishAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/withdraw")]
    [Authorize(Policy = "Broker")]
    public async Task<IActionResult> Withdraw(Guid id, CancellationToken cancellationToken)
    {
        await propertyService.WithdrawAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/sell")]
    [Authorize(Policy = "Broker")]
    public async Task<IActionResult> Sell(Guid id, CancellationToken cancellationToken)
    {
        await propertyService.MarkAsSoldAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Broker")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await propertyService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/images")]
    [Authorize(Policy = "Broker")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> AddImage(
        Guid id,
        IFormFile file,
        [FromForm] string altText,
        [FromForm] int sortOrder,
        [FromForm] bool isPrimary,
        CancellationToken cancellationToken)
    {
        await using var content = file.OpenReadStream();
        await propertyService.AddImageAsync(
            id,
            new AddPropertyImageRequest(content, file.FileName, file.ContentType, altText, sortOrder, isPrimary),
            cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}/images/{imageId:guid}")]
    [Authorize(Policy = "Broker")]
    public async Task<IActionResult> RemoveImage(Guid id, Guid imageId, CancellationToken cancellationToken)
    {
        await propertyService.RemoveImageAsync(id, imageId, cancellationToken);
        return NoContent();
    }
}