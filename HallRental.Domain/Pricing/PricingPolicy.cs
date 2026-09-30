using HallRental.Domain.Entities;
using HallRental.Domain.Exceptions;
using HallRental.Domain.ValueObjects;

namespace HallRental.Domain.Pricing;

// Zones must not overlap, so the standard 9–18 is split around the peak 12–14.
// Time outside every zone is outside working hours and can't be booked.
public sealed class PricingPolicy
{
    private readonly IReadOnlyList<HoursPricing> _zones;

    public PricingPolicy(IEnumerable<HoursPricing> zones)
    {
        _zones = zones.OrderBy(z => z.From).ToList();

        if (_zones.Count == 0)
            throw new DomainException("At least one hours pricing zone is required");

        EnsureNoOverlaps(_zones);
    }

    public decimal CalculateRent(decimal pricePerHour, RentalPeriod period)
    {
        decimal rent = 0;
        var coveredTime = TimeSpan.Zero;

        // A period may run past midnight, so zones are checked on every day it touches
        for (var day = period.Start.Date; day < period.End; day = day.AddDays(1))
        {
            foreach (var zone in _zones)
            {
                var start = Later(period.Start, day + zone.From.ToTimeSpan());
                var end = Earlier(period.End, day + zone.To.ToTimeSpan());
                if (start >= end)
                    continue;

                var timeInZone = end - start;
                rent += ToHours(timeInZone) * pricePerHour * zone.Multiplier;
                coveredTime += timeInZone;
            }
        }

        // Zones don't overlap, so this holds only when no minute is outside them
        if (coveredTime != period.Duration)
            throw new DomainException("The hall can be booked only during working hours");

        return decimal.Round(rent, 2, MidpointRounding.AwayFromZero);
    }

    // Sorted by start: if any two zones overlap, two neighbouring ones do too
    private static void EnsureNoOverlaps(IReadOnlyList<HoursPricing> sortedZones)
    {
        for (var i = 1; i < sortedZones.Count; i++)
        {
            if (sortedZones[i - 1].Overlaps(sortedZones[i]))
                throw new DomainException(
                    $"Hours pricing zones {sortedZones[i - 1].Name} and {sortedZones[i].Name} overlap");
        }
    }

    // Ticks keep the value exact, unlike TotalHours (double)
    private static decimal ToHours(TimeSpan time) => (decimal)time.Ticks / TimeSpan.TicksPerHour;

    private static DateTime Later(DateTime a, DateTime b) => a > b ? a : b;

    private static DateTime Earlier(DateTime a, DateTime b) => a < b ? a : b;
}
