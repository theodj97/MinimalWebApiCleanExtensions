using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MinimalWebApiCleanExtensions.Interfaces;

namespace MinimalWebApiCleanExtensions.Extensions;

public static class EndpointExtension
{
    public static IServiceCollection RegisterMinimalEndpoints(this IServiceCollection services,
                                                              Assembly assembly)
    {
        var endpointTypes = assembly.GetTypes().Where(t => typeof(IGroupMap).IsAssignableFrom(t)
                && !t.IsInterface
                && !t.IsAbstract);

        foreach (var type in endpointTypes)
            services.TryAddEnumerable(ServiceDescriptor.Transient(typeof(IGroupMap), type));

        return services;
    }

    public static IApplicationBuilder MapRoutes(this WebApplication app)
    {
        IEnumerable<IGroupMap> endpoints = app.Services.GetRequiredService<IEnumerable<IGroupMap>>();

        foreach (IGroupMap endpoint in endpoints)
            endpoint.MapGroup(app);

        return app;
    }
}