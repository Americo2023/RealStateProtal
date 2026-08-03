using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using RealStatePortal.Application.Abstractions.Persistence;
using RealStatePortal.Infrastructure.Persistence;
using RealStatePortal.Infrastructure.Persistence.Repositories;
using RealStatePortal.Application.Abstractions.Storage;
using RealStatePortal.Application.Abstractions.Time;
using RealStatePortal.Infrastructure.Storage;
using RealStatePortal.Infrastructure.Time;
using RealStatePortal.Application.Abstractions.Email;
using RealStatePortal.Infrastructure.Email;
using RealStatePortal.Application.Abstractions.Authentication;
using RealStatePortal.Infrastructure.Authentication;

namespace RealStatePortal.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")?
            .Replace("${MSSQL_SA_PASSWORD}", Environment.GetEnvironmentVariable("MSSQL_SA_PASSWORD") ?? string.Empty, StringComparison.Ordinal);
        services.AddDbContext<RealStatePortalDbContext>(options =>
            options.UseSqlServer(connectionString));
        services.AddScoped<IPropertyRepository, PropertyRepository>();
        services.AddScoped<IFavoriteRepository, FavoriteRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBrokerProfileRepository, BrokerProfileRepository>();
        services.AddScoped<IContactInquiryRepository, ContactInquiryRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        services.AddSingleton<IImageStorage, LocalImageStorage>();
        services.AddTransient<IEmailSender, SmtpEmailSender>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        return services;
    }
}
