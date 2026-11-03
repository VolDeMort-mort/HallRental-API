using HallRental.Bll.Common.Pricing.Models;

namespace HallRental.Bll.Tests.Shared;

// The same zones as the database seed: working hours 06:00–23:00
internal static class TestZones
{
    public static IReadOnlyList<HoursPricing> Default() =>
    [
        Zone("Morning", 6, 9, 0.90m),
        Zone("Standard", 9, 12, 1.00m),
        Zone("Peak", 12, 14, 1.15m),
        Zone("Standard", 14, 18, 1.00m),
        Zone("Evening", 18, 23, 0.80m)
    ];

    public static HoursPricing Zone(string name, int fromHour, int toHour, decimal multiplier) => new()
    {
        Id = Guid.NewGuid(),
        Name = name,
        From = new TimeOnly(fromHour, 0),
        To = new TimeOnly(toHour, 0),
        Multiplier = multiplier
    };
}
