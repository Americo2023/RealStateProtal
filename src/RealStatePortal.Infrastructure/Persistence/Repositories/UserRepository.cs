using Microsoft.EntityFrameworkCore;
using RealStatePortal.Application.Abstractions.Persistence;
using RealStatePortal.Domain.Entities;
using RealStatePortal.Domain.Enums;

namespace RealStatePortal.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(RealStatePortalDbContext dbContext) : IUserRepository
{
    public async Task<IReadOnlyCollection<User>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Users.AsNoTracking().OrderBy(user => user.Email).ToArrayAsync(cancellationToken);

    public Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        dbContext.Users.AsNoTracking().SingleOrDefaultAsync(user => user.Id == userId, cancellationToken);

    public Task<User?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken = default) =>
        dbContext.Users.SingleOrDefaultAsync(user => user.Auth0UserId == externalId, cancellationToken);

    public async Task<IReadOnlyCollection<UserRole>> GetRolesAsync(
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await dbContext.UserRoles
            .Where(role => role.UserId == userId)
            .Select(role => role.Role)
            .ToArrayAsync(cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken = default) =>
        await dbContext.Users.AddAsync(user, cancellationToken);

    public async Task ReplaceRolesAsync(
        Guid userId,
        IReadOnlyCollection<UserRole> roles,
        CancellationToken cancellationToken = default)
    {
        var currentRoles = await dbContext.UserRoles
            .Where(role => role.UserId == userId)
            .ToListAsync(cancellationToken);

        dbContext.UserRoles.RemoveRange(currentRoles);
        await dbContext.UserRoles.AddRangeAsync(
            roles.Distinct().Select(role => new UserRoleAssignment(userId, role)),
            cancellationToken);
    }
}