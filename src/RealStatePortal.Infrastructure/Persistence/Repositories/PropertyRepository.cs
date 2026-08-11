using Microsoft.EntityFrameworkCore;
using RealStatePortal.Application.Abstractions.Persistence;
using RealStatePortal.Domain.Entities;

namespace RealStatePortal.Infrastructure.Persistence.Repositories;

public sealed class PropertyRepository(RealStatePortalDbContext dbContext) : IPropertyRepository
{
    public Task<Property?> GetByIdAsync(Guid propertyId, CancellationToken cancellationToken = default) =>
        Query().SingleOrDefaultAsync(property => property.Id == propertyId, cancellationToken);

    public Task<bool> ExistsByReferenceNumberAsync(string referenceNumber, CancellationToken cancellationToken = default) =>
        dbContext.Properties.AnyAsync(property => property.ReferenceNumber == referenceNumber, cancellationToken);

    public async Task AddAsync(Property aggregate, CancellationToken cancellationToken = default)
    {
        await dbContext.Properties.AddAsync(aggregate, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Property>> GetPublishedAsync(CancellationToken cancellationToken = default) =>
        await Query()
            .Where(property => property.Status == Domain.Enums.PropertyStatus.Published)
            .OrderByDescending(property => property.PublishedAt)
            .ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyCollection<Property>> SearchAsync(string? query, CancellationToken cancellationToken = default)
    {
        var properties = Query().Where(property => property.Status == Domain.Enums.PropertyStatus.Published);

        if (!string.IsNullOrWhiteSpace(query))
        {
            var normalizedQuery = query.Trim();
            properties = properties.Where(property =>
                property.Title.Contains(normalizedQuery) ||
                property.Description.Contains(normalizedQuery) ||
                (property.Address != null && (property.Address.City.Contains(normalizedQuery) ||
                                               property.Address.PostalCode.Contains(normalizedQuery) ||
                                               property.Address.Street.Contains(normalizedQuery))));
        }

        return await properties.OrderByDescending(property => property.CreatedAt).ToArrayAsync(cancellationToken);
    }

    private IQueryable<Property> Query() => dbContext.Properties
        .AsNoTracking()
        .Include(property => property.Address)
        .Include(property => property.Images);
}