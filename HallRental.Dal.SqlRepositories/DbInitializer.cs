using HallRental.Bll.Common.Shared;
using HallRental.Dal.SqlRepositories.Halls.Entities;
using HallRental.Dal.SqlRepositories.Pricing.Entities;
using Microsoft.EntityFrameworkCore;

namespace HallRental.Dal.SqlRepositories;

internal class DbInitializer : IDatabaseInitializer
{
    private readonly AppDbContext _context;

    public DbInitializer(AppDbContext context)
    {
        _context = context;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        await _context.Database.MigrateAsync(cancellationToken);

        // Deleted halls count too, otherwise seeding again would bring them back
        if (!await _context.Halls.IgnoreQueryFilters().AnyAsync(cancellationToken))
            _context.Halls.AddRange(CreateHalls());

        if (!await _context.HoursPricings.AnyAsync(cancellationToken))
            _context.HoursPricings.AddRange(CreateHoursPricings());

        await _context.SaveChangesAsync(cancellationToken);
    }

    private static IEnumerable<HallEntity> CreateHalls()
    {
        var halls = new[]
        {
            CreateHall("Зал A", 50, 2000m),
            CreateHall("Зал B", 100, 3500m),
            CreateHall("Зал C", 30, 1500m)
        };

        // The assignment doesn't say which hall offers which service, so every hall offers all of them
        foreach (var hall in halls)
        {
            hall.Services.Add(CreateService(hall.Id, "Проєктор", 500m));
            hall.Services.Add(CreateService(hall.Id, "Wi-Fi", 300m));
            hall.Services.Add(CreateService(hall.Id, "Звук", 700m));
        }

        return halls;
    }

    // Peak hours (12–14) lie inside the standard ones, so the standard zone is split around them
    private static IEnumerable<HoursPricingEntity> CreateHoursPricings() =>
    [
        CreateZone("Morning", new TimeOnly(6, 0), new TimeOnly(9, 0), 0.90m),
        CreateZone("Standard", new TimeOnly(9, 0), new TimeOnly(12, 0), 1.00m),
        CreateZone("Peak", new TimeOnly(12, 0), new TimeOnly(14, 0), 1.15m),
        CreateZone("Standard", new TimeOnly(14, 0), new TimeOnly(18, 0), 1.00m),
        CreateZone("Evening", new TimeOnly(18, 0), new TimeOnly(23, 0), 0.80m)
    ];

    private static HallEntity CreateHall(string name, int capacity, decimal pricePerHour) => new()
    {
        Id = Guid.NewGuid(),
        Name = name,
        Capacity = capacity,
        PricePerHour = pricePerHour,
        IsActive = true
    };

    private static ServiceEntity CreateService(Guid hallId, string name, decimal price) => new()
    {
        Id = Guid.NewGuid(),
        HallId = hallId,
        Name = name,
        Price = price
    };

    private static HoursPricingEntity CreateZone(string name, TimeOnly from, TimeOnly to, decimal multiplier) => new()
    {
        Id = Guid.NewGuid(),
        Name = name,
        From = from,
        To = to,
        Multiplier = multiplier
    };
}
