using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RealStatePortal.Infrastructure.Persistence;

public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<RealStatePortalDbContext>
{
    public RealStatePortalDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Server=localhost,1433;Database=RealStatePortal;User Id=sa;Password=PLACEHOLDER;TrustServerCertificate=True;";
        var optionsBuilder = new DbContextOptionsBuilder<RealStatePortalDbContext>();
        optionsBuilder.UseSqlServer(connectionString);
        return new RealStatePortalDbContext(optionsBuilder.Options);
    }
}