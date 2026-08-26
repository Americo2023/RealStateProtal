using Microsoft.EntityFrameworkCore;
using RealStatePortal.Application.Abstractions.Persistence;
using RealStatePortal.Application.ContactInquiries;
using RealStatePortal.Domain.Entities;

namespace RealStatePortal.Infrastructure.Persistence.Repositories;

public sealed class ContactInquiryRepository(RealStatePortalDbContext dbContext) : IContactInquiryRepository
{
    public async Task AddAsync(ContactInquiry inquiry, CancellationToken cancellationToken = default) =>
        await dbContext.ContactInquiries.AddAsync(inquiry, cancellationToken);

    public async Task<IReadOnlyCollection<ContactInquiryDto>> GetByBrokerIdAsync(
        Guid brokerId,
        CancellationToken cancellationToken = default) =>
        await dbContext.ContactInquiries
            .AsNoTracking()
            .Join(
                dbContext.Properties,
                inquiry => inquiry.PropertyId,
                property => property.Id,
                (inquiry, property) => new { inquiry, property })
            .Where(item => item.property.BrokerId == brokerId)
            .OrderByDescending(item => item.inquiry.CreatedAt)
            .Select(item => new ContactInquiryDto(
                item.inquiry.Id,
                item.inquiry.PropertyId,
                item.property.Title,
                item.property.ReferenceNumber,
                item.inquiry.VisitorName,
                item.inquiry.VisitorEmail,
                item.inquiry.VisitorPhone,
                item.inquiry.Message,
                item.inquiry.CreatedAt))
            .ToArrayAsync(cancellationToken);
}