using RealStatePortal.Domain.Entities;
using RealStatePortal.Application.Properties;

namespace RealStatePortal.Application.Abstractions.Persistence;

public interface IPropertyRepository
{
    Task<Property?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Property>> SearchAsync(PropertySearchCriteria criteria, CancellationToken cancellationToken = default);
    void Add(Property property);
    void Remove(Property property);
}