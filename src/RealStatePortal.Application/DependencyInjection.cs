using Microsoft.Extensions.DependencyInjection;
using RealStatePortal.Application.ContactInquiries;
using RealStatePortal.Application.Favorites;
using RealStatePortal.Application.Properties.Services;
using RealStatePortal.Application.Abstractions.Authentication;
using RealStatePortal.Application.Users;

namespace RealStatePortal.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPropertyService, PropertyService>();
        services.AddScoped<IFavoriteService, FavoriteService>();
        services.AddScoped<IContactInquiryService, ContactInquiryService>();
        services.AddScoped<IIdentityProvisioningService, IdentityProvisioningService>();
        services.AddScoped<IUserService, UserService>();
        return services;
    }
}