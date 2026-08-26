using RealStatePortal.Application.Common;

namespace RealStatePortal.Application.ContactInquiries;

public interface IContactInquiryService
{
    Task<Result> CreateAsync(CreateContactInquiryRequest request, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<ContactInquiryDto>>> GetMineAsync(CancellationToken cancellationToken = default);
}