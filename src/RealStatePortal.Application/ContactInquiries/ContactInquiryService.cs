using RealStatePortal.Application.Abstractions.Email;
using RealStatePortal.Application.Abstractions.Persistence;
using RealStatePortal.Application.Abstractions.Time;
using RealStatePortal.Application.Common;
using RealStatePortal.Domain.Entities;

namespace RealStatePortal.Application.ContactInquiries;

public sealed class ContactInquiryService(
    IPropertyRepository propertyRepository,
    IBrokerRepository brokerRepository,
    IContactInquiryRepository inquiryRepository,
    IEmailSender emailSender,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider) : IContactInquiryService
{
    public async Task<Result> CreateAsync(CreateContactInquiryRequest request, CancellationToken cancellationToken = default)
    {
        var property = await propertyRepository.GetByIdAsync(request.PropertyId, cancellationToken);
        if (property is null || property.Status != Domain.Enums.PropertyStatus.Published)
        {
            return Result.Failure("Contact inquiries require a published property.");
        }

        var broker = await brokerRepository.GetByPropertyIdAsync(property.Id, cancellationToken);
        if (broker is null || !broker.IsActive)
        {
            return Result.Failure("The property broker is unavailable.");
        }

        var inquiry = new ContactInquiry(
            property.Id,
            request.VisitorName,
            request.VisitorEmail,
            request.VisitorPhone,
            request.Message,
            dateTimeProvider.UtcNow);

        await inquiryRepository.AddAsync(inquiry, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await emailSender.SendAsync(
            new EmailMessage(
                broker.Email,
                $"Property inquiry: {property.ReferenceNumber}",
                request.Message,
                request.VisitorEmail),
            cancellationToken);

        return Result.Success();
    }
}