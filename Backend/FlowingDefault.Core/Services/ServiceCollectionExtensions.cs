using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace FlowingDefault.Core.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructuralServices(this IServiceCollection services)
    {
        // Get all types in the current assembly that are in the FlowingDefault.Core.Services namespace
        var serviceTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(type => type.Namespace == "FlowingDefault.Core.Services" && 
                          !type.IsAbstract && 
                          !type.IsInterface && 
                          type.IsClass);

        // Register each service type as scoped
        foreach (var serviceType in serviceTypes)
            services.AddScoped(serviceType);

        return services;
    }
}