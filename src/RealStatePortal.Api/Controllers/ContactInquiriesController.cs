using Microsoft.AspNetCore.Mvc;
using RealStatePortal.Application.ContactInquiries;

namespace RealStatePortal.Api.Controllers;

[ApiController]
[Route("api/contact-inquiries")]
public sealed class ContactInquiriesController(IContactInquiryService inquiryService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateContactInquiryRequest request, CancellationToken cancellationToken)
    {
        await inquiryService.CreateAsync(request, cancellationToken);
        return Accepted();
    }
}