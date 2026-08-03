using Microsoft.EntityFrameworkCore;
using RealStatePortal.Application.Abstractions.Persistence;
using RealStatePortal.Application.Properties;
using RealStatePortal.Domain.Entities;
using RealStatePortal.Domain.Enums;

namespace RealStatePortal.Infrastructure.Persistence.Repositories;

public sealed class PropertyRepository(RealStatePortalDbContext dbContext) : IPropertyRepository
{
    public Task<Property?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Properties
            .Include(property => property.Address)
            .Include(property => property.Images)
            .SingleOrDefaultAsync(property => property.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<Property>> SearchAsync(
        PropertySearchCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Properties
            .AsNoTracking()
            .Include(property => property.Address)
            .Include(property => property.Images)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(criteria.SearchText))
        {
            var text = criteria.SearchText.Trim();
            query = query.Where(property =>
                property.Title.Contains(text) ||
                property.Description.Contains(text) ||
                (property.Address != null &&
                 (property.Address.City.Contains(text) ||
                  property.Address.PostalCode.Contains(text) ||
                  property.Address.Street.Contains(text))));
        }

        if (criteria.PropertyType.HasValue)
        {
            query = query.Where(property => property.PropertyType == criteria.PropertyType.Value);
        }

        if (!string.IsNullOrWhiteSpace(criteria.City))
        {
            var city = criteria.City.Trim();
            query = query.Where(property => property.Address != null && property.Address.City == city);
        }

        if (criteria.PriceMin.HasValue)
        {
            query = query.Where(property => property.Price.Amount >= criteria.PriceMin.Value);
        }

        if (criteria.PriceMax.HasValue)
        {
            query = query.Where(property => property.Price.Amount <= criteria.PriceMax.Value);
        }

        if (criteria.BedroomsMin.HasValue)
        {
            query = query.Where(property => property.Bedrooms >= criteria.BedroomsMin.Value);
        }

        if (criteria.BathroomsMin.HasValue)
        {
            query = query.Where(property => property.Bathrooms >= criteria.BathroomsMin.Value);
        }

        if (criteria.AreaMin.HasValue)
        {
            query = query.Where(property => property.LivingArea >= criteria.AreaMin.Value);
        }

        if (criteria.AreaMax.HasValue)
        {
            query = query.Where(property => property.LivingArea <= criteria.AreaMax.Value);
        }

        if (criteria.Status.HasValue)
        {
            query = query.Where(property => property.Status == criteria.Status.Value);
        }
        else
        {
            query = query.Where(property => property.Status == PropertyStatus.Published);
        }

        query = criteria.SortOrder switch
        {
            PropertySortOrder.Oldest => query.OrderBy(property => property.CreatedAt),
            PropertySortOrder.PriceLowToHigh => query.OrderBy(property => property.Price.Amount),
            PropertySortOrder.PriceHighToLow => query.OrderByDescending(property => property.Price.Amount),
            _ => query.OrderByDescending(property => property.CreatedAt)
        };

        var pageNumber = Math.Max(criteria.PageNumber, 1);
        var pageSize = Math.Clamp(criteria.PageSize, 1, 100);
        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);
    }

    public void Add(Property property) => dbContext.Properties.Add(property);

    public void Remove(Property property) => dbContext.Properties.Remove(property);
}

public sealed class FavoriteRepository(RealStatePortalDbContext dbContext) : IFavoriteRepository
{
    public async Task<IReadOnlyCollection<Favorite>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await dbContext.Favorites.AsNoTracking().Where(favorite => favorite.UserId == userId).OrderByDescending(favorite => favorite.CreatedAt).ToArrayAsync(cancellationToken);

    public Task<bool> ExistsAsync(Guid userId, Guid propertyId, CancellationToken cancellationToken = default) =>
        dbContext.Favorites.AnyAsync(favorite => favorite.UserId == userId && favorite.PropertyId == propertyId, cancellationToken);

    public Task<Favorite?> GetAsync(Guid userId, Guid propertyId, CancellationToken cancellationToken = default) =>
        dbContext.Favorites.SingleOrDefaultAsync(favorite => favorite.UserId == userId && favorite.PropertyId == propertyId, cancellationToken);

    public void Add(Favorite favorite) => dbContext.Favorites.Add(favorite);

    public void Remove(Favorite favorite) => dbContext.Favorites.Remove(favorite);
}

public sealed class UserRepository(RealStatePortalDbContext dbContext) : IUserRepository
{
    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Users.SingleOrDefaultAsync(user => user.Id == id, cancellationToken);

    public Task<User?> GetByAuth0UserIdAsync(string auth0UserId, CancellationToken cancellationToken = default) =>
        dbContext.Users.SingleOrDefaultAsync(user => user.Auth0UserId == auth0UserId, cancellationToken);

    public async Task<IReadOnlyCollection<User>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Users.AsNoTracking().OrderBy(user => user.LastName).ThenBy(user => user.FirstName).ToArrayAsync(cancellationToken);

    public void Add(User user) => dbContext.Users.Add(user);
}

public sealed class BrokerProfileRepository(RealStatePortalDbContext dbContext) : IBrokerProfileRepository
{
    public Task<BrokerProfile?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        dbContext.BrokerProfiles.SingleOrDefaultAsync(profile => profile.UserId == userId, cancellationToken);

    public async Task<IReadOnlyCollection<BrokerProfile>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.BrokerProfiles.AsNoTracking().OrderBy(profile => profile.FullName).ToArrayAsync(cancellationToken);

    public void Add(BrokerProfile brokerProfile) => dbContext.BrokerProfiles.Add(brokerProfile);
}

public sealed class ContactInquiryRepository(RealStatePortalDbContext dbContext) : IContactInquiryRepository
{
    public void Add(ContactInquiry inquiry) => dbContext.ContactInquiries.Add(inquiry);
}

public sealed class AuditLogRepository(RealStatePortalDbContext dbContext) : IAuditLogRepository
{
    public async Task<IReadOnlyCollection<AuditLog>> GetByEntityAsync(string entityName, Guid entityId, CancellationToken cancellationToken = default) =>
        await dbContext.AuditLogs.AsNoTracking().Where(log => log.EntityName == entityName && log.EntityId == entityId).OrderByDescending(log => log.ChangedAt).ToArrayAsync(cancellationToken);

    public void Add(AuditLog auditLog) => dbContext.AuditLogs.Add(auditLog);
}

public sealed class UnitOfWork(RealStatePortalDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);
}