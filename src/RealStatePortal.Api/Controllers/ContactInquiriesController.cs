using Microsoft.AspNetCore.Mvc;
using RealStatePortal.Application.ContactInquiries;

namespace RealStatePortal.Api.Controllers;

[Route("api/contact-inquiries")]
public sealed class ContactInquiriesController(IContactInquiryService inquiryService) : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateContactInquiryRequest request,
        CancellationToken cancellationToken)
    {
        var result = await inquiryService.CreateAsync(request, cancellationToken);
        return FromResult(result);
    }
}