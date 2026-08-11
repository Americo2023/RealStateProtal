using RealStatePortal.Application.Abstractions.Persistence;
using RealStatePortal.Domain.Entities;

namespace RealStatePortal.Infrastructure.Persistence.Repositories;

public sealed class ContactInquiryRepository(RealStatePortalDbContext dbContext) : IContactInquiryRepository
{
    public async Task AddAsync(ContactInquiry inquiry, CancellationToken cancellationToken = default) =>
        await dbContext.ContactInquiries.AddAsync(inquiry, cancellationToken);
}