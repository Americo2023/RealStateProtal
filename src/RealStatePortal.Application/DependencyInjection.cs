using Microsoft.Extensions.DependencyInjection;
using RealStatePortal.Application.Auditing;
using RealStatePortal.Application.Brokers;
using RealStatePortal.Application.ContactInquiries;
using RealStatePortal.Application.Favorites;
using RealStatePortal.Application.Properties;
using RealStatePortal.Application.Users;

namespace RealStatePortal.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPropertyService, PropertyService>();
        services.AddScoped<IFavoriteService, FavoriteService>();
        services.AddScoped<IContactInquiryService, ContactInquiryService>();
        services.AddScoped<IUserAdministrationService, UserAdministrationService>();
        services.AddScoped<IBrokerAdministrationService, BrokerAdministrationService>();
        services.AddScoped<IAuditService, AuditService>();
        return services;
    }
}
