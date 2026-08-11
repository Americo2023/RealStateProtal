using RealStatePortal.Domain.Entities;

namespace RealStatePortal.Application.Abstractions.Persistence;

public interface IContactInquiryRepository
{
    Task AddAsync(ContactInquiry inquiry, CancellationToken cancellationToken = default);
}