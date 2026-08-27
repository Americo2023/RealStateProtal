using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealStatePortal.Application.ContactInquiries;

namespace RealStatePortal.Api.Controllers;

[Route("api/contact-inquiries")]
public sealed class ContactInquiriesController(IContactInquiryService inquiryService) : ApiControllerBase
{
    [HttpGet("mine")]
    [Authorize(Policy = "BrokerOrAdministrator")]
    public async Task<IActionResult> GetMine(CancellationToken cancellationToken)
    {
        var result = await inquiryService.GetMineAsync(cancellationToken);
        return FromResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateContactInquiryRequest request,
        CancellationToken cancellationToken)
    {
        var result = await inquiryService.CreateAsync(request, cancellationToken);
        return FromResult(result);
    }
}