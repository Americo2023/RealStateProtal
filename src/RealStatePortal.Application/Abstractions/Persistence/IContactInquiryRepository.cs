using RealStatePortal.Application.ContactInquiries;
using RealStatePortal.Domain.Entities;

namespace RealStatePortal.Application.Abstractions.Persistence;

public interface IContactInquiryRepository
{
    Task AddAsync(ContactInquiry inquiry, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ContactInquiryDto>> GetByBrokerIdAsync(Guid brokerId, CancellationToken cancellationToken = default);
}