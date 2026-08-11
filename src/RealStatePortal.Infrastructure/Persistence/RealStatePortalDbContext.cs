using Microsoft.EntityFrameworkCore;
using RealStatePortal.Domain.Entities;

namespace RealStatePortal.Infrastructure.Persistence;

public sealed class RealStatePortalDbContext(DbContextOptions<RealStatePortalDbContext> options) : DbContext(options)
{
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<PropertyAddress> PropertyAddresses => Set<PropertyAddress>();
    public DbSet<PropertyImage> PropertyImages => Set<PropertyImage>();
    public DbSet<Favorite> Favorites => Set<Favorite>();
    public DbSet<User> Users => Set<User>();
    public DbSet<BrokerProfile> BrokerProfiles => Set<BrokerProfile>();
    public DbSet<ContactInquiry> ContactInquiries => Set<ContactInquiry>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RealStatePortalDbContext).Assembly);
    }
}