namespace RealStatePortal.Application.ContactInquiries;

public interface IContactInquiryService
{
    Task CreateAsync(CreateContactInquiryRequest request, CancellationToken cancellationToken = default);
}