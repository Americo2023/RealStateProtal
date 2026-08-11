using RealStatePortal.Domain.Entities;

namespace RealStatePortal.Application.Abstractions.Persistence;

public interface IPropertyRepository
{
    Task<Property?> GetByIdAsync(Guid propertyId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByReferenceNumberAsync(string referenceNumber, CancellationToken cancellationToken = default);
    Task AddAsync(Property aggregate, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Property>> GetPublishedAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Property>> SearchAsync(string? query, CancellationToken cancellationToken = default);
}