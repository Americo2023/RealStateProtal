using RealStatePortal.Domain.Entities;

namespace RealStatePortal.Application.Abstractions.Persistence;

public interface IContactInquiryRepository
{
    void Add(ContactInquiry inquiry);
}