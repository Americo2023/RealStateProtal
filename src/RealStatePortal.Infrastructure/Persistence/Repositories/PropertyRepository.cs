using Microsoft.EntityFrameworkCore;
using RealStatePortal.Application.Abstractions.Persistence;
using RealStatePortal.Application.Properties.Dtos;
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

    public async Task<IReadOnlyCollection<Property>> GetByBrokerIdAsync(Guid brokerId, CancellationToken cancellationToken = default) =>
        await Query()
            .Where(property => property.BrokerId == brokerId && property.Status != Domain.Enums.PropertyStatus.Deleted)
            .OrderByDescending(property => property.UpdatedAt)
            .ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyCollection<Property>> SearchAsync(PropertySearchRequest request, CancellationToken cancellationToken = default)
    {
        var properties = Query().Where(property => property.Status == Domain.Enums.PropertyStatus.Published);

        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            var normalizedQuery = request.Query.Trim();
            properties = properties.Where(property =>
                property.Title.Contains(normalizedQuery) ||
                property.Description.Contains(normalizedQuery) ||
                (property.Address != null && (property.Address.City.Contains(normalizedQuery) ||
                                               property.Address.PostalCode.Contains(normalizedQuery) ||
                                               property.Address.Street.Contains(normalizedQuery))));
        }

        if (!string.IsNullOrWhiteSpace(request.City))
            properties = properties.Where(property => property.Address != null && property.Address.City.Contains(request.City));
        if (request.PropertyType.HasValue) properties = properties.Where(property => property.PropertyType == request.PropertyType);
        if (request.PriceMin.HasValue) properties = properties.Where(property => property.Price.Amount >= request.PriceMin);
        if (request.PriceMax.HasValue) properties = properties.Where(property => property.Price.Amount <= request.PriceMax);
        if (request.BedroomsMin.HasValue) properties = properties.Where(property => property.Bedrooms >= request.BedroomsMin);
        if (request.BathroomsMin.HasValue) properties = properties.Where(property => property.Bathrooms >= request.BathroomsMin);
        if (request.AreaMin.HasValue) properties = properties.Where(property => property.LivingArea >= request.AreaMin);
        if (request.AreaMax.HasValue) properties = properties.Where(property => property.LivingArea <= request.AreaMax);
        if (request.Status.HasValue) properties = properties.Where(property => property.Status == request.Status);

        properties = request.Sort switch
        {
            "Oldest" => properties.OrderBy(property => property.CreatedAt),
            "PriceLowToHigh" => properties.OrderBy(property => property.Price.Amount),
            "PriceHighToLow" => properties.OrderByDescending(property => property.Price.Amount),
            _ => properties.OrderByDescending(property => property.CreatedAt)
        };
        return await properties.ToArrayAsync(cancellationToken);
    }

    private IQueryable<Property> Query() => dbContext.Properties
        .AsNoTracking()
        .Include(property => property.Address)
        .Include(property => property.Images);
}