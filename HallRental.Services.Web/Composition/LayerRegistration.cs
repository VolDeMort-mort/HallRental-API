using System.Reflection;
using HallRental.Bll.Common.Shared;

namespace HallRental.Services.Web.Composition;

internal static class LayerRegistration
{
    // By name, not by type: the web project has no compile-time reference to the DAL
    public static IServiceCollection AddLayers(this IServiceCollection services, IConfiguration configuration, params string[] assemblyNames)
    {
        foreach (var assemblyName in assemblyNames)
        {
            var registrars = Assembly.Load(assemblyName).GetExportedTypes()
                .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(IDependencyRegistrar).IsAssignableFrom(t));

            foreach (var type in registrars)
            {
                var registrar = (IDependencyRegistrar)Activator.CreateInstance(type)!;
                registrar.Register(services, configuration);
            }
        }

        return services;
    }
}
