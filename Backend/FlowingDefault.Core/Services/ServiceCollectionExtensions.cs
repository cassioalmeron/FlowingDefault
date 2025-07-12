using Microsoft.Extensions.DependencyInjection;

namespace FlowingDefault.Core.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructuralServices(this IServiceCollection services)
    {
        services.AddScoped<LoginService>();
        services.AddScoped<ProjectService>();
        services.AddScoped<UserService>();
        services.AddScoped<ProfileService>();
        services.AddScoped<LabelService>();

        return services;
    }
}