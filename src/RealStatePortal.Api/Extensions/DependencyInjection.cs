using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealStatePortal.Application;
using RealStatePortal.Infrastructure;

namespace RealStatePortal.Api.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddProjectServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddApplication();
        services.AddInfrastructure(configuration);
        return services;
    }
}
