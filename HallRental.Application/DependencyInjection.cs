using FluentValidation;
using HallRental.Application.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace HallRental.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Registers every handler and validator of this assembly, so new use cases need no changes in Program.cs.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly);

        // Without this, messages follow the machine's language: Ukrainian on a dev PC, English on a Linux server
        ValidatorOptions.Global.LanguageManager.Enabled = false;

        return services;
    }
}
