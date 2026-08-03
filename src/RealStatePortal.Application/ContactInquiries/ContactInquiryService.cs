using RealStatePortal.Application.Abstractions.Email;
using RealStatePortal.Application.Abstractions.Persistence;
using RealStatePortal.Application.Abstractions.Time;
using RealStatePortal.Domain.Entities;
using RealStatePortal.Domain.Enums;

namespace RealStatePortal.Application.ContactInquiries;

public sealed class ContactInquiryService(
    IContactInquiryRepository inquiryRepository,
    IPropertyRepository propertyRepository,
    IBrokerProfileRepository brokerProfileRepository,
    IEmailSender emailSender,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider) : IContactInquiryService
{
    public async Task CreateAsync(CreateContactInquiryRequest request, CancellationToken cancellationToken = default)
    {
        var property = await propertyRepository.GetByIdAsync(request.PropertyId, cancellationToken)
            ?? throw new KeyNotFoundException($"Property '{request.PropertyId}' was not found.");

        if (property.Status != PropertyStatus.Published)
        {
            throw new InvalidOperationException("Contact inquiries can only be created for published properties.");
        }

        var broker = await brokerProfileRepository.GetByIdAsync(property.BrokerId, cancellationToken)
            ?? throw new InvalidOperationException("The property broker profile was not found.");

        var inquiry = new ContactInquiry(
            Guid.NewGuid(),
            request.PropertyId,
            request.VisitorName,
            request.VisitorEmail,
            request.VisitorPhone,
            request.Message,
            dateTimeProvider.UtcNow);

        inquiryRepository.Add(inquiry);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await emailSender.SendAsync(
            new EmailMessage(
                broker.Email,
                $"New inquiry for property {property.ReferenceNumber}",
                $"{inquiry.VisitorName} ({inquiry.VisitorEmail}) sent: {inquiry.Message}",
                [inquiry.VisitorEmail]),
            cancellationToken);
    }
}