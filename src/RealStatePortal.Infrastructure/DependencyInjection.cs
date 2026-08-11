using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealStatePortal.Application.Abstractions.Authentication;
using RealStatePortal.Application.Abstractions.Email;
using RealStatePortal.Application.Abstractions.Persistence;
using RealStatePortal.Application.Abstractions.Storage;
using RealStatePortal.Application.Abstractions.Time;
using RealStatePortal.Infrastructure.Authentication;
using RealStatePortal.Infrastructure.Email;
using RealStatePortal.Infrastructure.Persistence;
using RealStatePortal.Infrastructure.Persistence.Repositories;
using RealStatePortal.Infrastructure.Storage;

namespace RealStatePortal.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is required.");

        services.AddDbContext<RealStatePortalDbContext>(options => options.UseSqlServer(connectionString));
        services.AddHttpContextAccessor();
        services.AddScoped<IPropertyRepository, PropertyRepository>();
        services.AddScoped<IFavoriteRepository, FavoriteRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IContactInquiryRepository, ContactInquiryRepository>();
        services.AddScoped<IBrokerRepository, BrokerRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        services.AddScoped<IEmailSender, SmtpEmailSender>();
        services.AddScoped<IImageStorage, LocalImageStorage>();
        return services;
    }
}