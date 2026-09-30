using HallRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HallRental.Infrastructure.Persistence;

// Seed data goes through the domain factories, so it passes the same checks as data from the API
public static class DbInitializer
{
    public static async Task InitializeDatabaseAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await context.Database.MigrateAsync(cancellationToken);

        // Deleted halls count too, otherwise seeding again would bring them back
        if (!await context.Halls.IgnoreQueryFilters().AnyAsync(cancellationToken))
            context.Halls.AddRange(CreateHalls());

        if (!await context.HoursPricings.AnyAsync(cancellationToken))
            context.HoursPricings.AddRange(CreateHoursPricings());

        await context.SaveChangesAsync(cancellationToken);
    }

    private static IEnumerable<Hall> CreateHalls()
    {
        var halls = new[]
        {
            Hall.Create("Зал A", 50, 2000m),
            Hall.Create("Зал B", 100, 3500m),
            Hall.Create("Зал C", 30, 1500m)
        };

        // The assignment doesn't say which hall offers which service, so every hall offers all of them
        foreach (var hall in halls)
        {
            hall.AddService(Service.Create("Проєктор", 500m));
            hall.AddService(Service.Create("Wi-Fi", 300m));
            hall.AddService(Service.Create("Звук", 700m));
        }

        return halls;
    }

    // Peak hours (12–14) lie inside the standard ones, so the standard zone is split around them
    private static IEnumerable<HoursPricing> CreateHoursPricings() =>
    [
        HoursPricing.Create("Morning", new TimeOnly(6, 0), new TimeOnly(9, 0), 0.90m),
        HoursPricing.Create("Standard", new TimeOnly(9, 0), new TimeOnly(12, 0), 1.00m),
        HoursPricing.Create("Peak", new TimeOnly(12, 0), new TimeOnly(14, 0), 1.15m),
        HoursPricing.Create("Standard", new TimeOnly(14, 0), new TimeOnly(18, 0), 1.00m),
        HoursPricing.Create("Evening", new TimeOnly(18, 0), new TimeOnly(23, 0), 0.80m)
    ];
}
