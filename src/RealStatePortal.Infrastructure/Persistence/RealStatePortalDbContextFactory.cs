using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RealStatePortal.Infrastructure.Persistence;

public sealed class RealStatePortalDbContextFactory : IDesignTimeDbContextFactory<RealStatePortalDbContext>
{
    public RealStatePortalDbContext CreateDbContext(string[] args)
    {
        var password = Environment.GetEnvironmentVariable("MSSQL_SA_PASSWORD")
            ?? throw new InvalidOperationException("MSSQL_SA_PASSWORD must be configured for EF Core tooling.");
        var optionsBuilder = new DbContextOptionsBuilder<RealStatePortalDbContext>();
        optionsBuilder.UseSqlServer(
            $"Server=localhost,1433;Database=RealStatePortal;User Id=sa;Password={password};TrustServerCertificate=True;");
        return new RealStatePortalDbContext(optionsBuilder.Options);
    }
}