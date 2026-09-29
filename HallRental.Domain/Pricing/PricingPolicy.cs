using HallRental.Domain.Entities;
using HallRental.Domain.Exceptions;
using HallRental.Domain.ValueObjects;

namespace HallRental.Domain.Pricing;

/// <summary>
/// Calculates the rent of a hall for a period using time-of-day tariff zones.
/// <para>
/// Zones must not overlap, so every moment has at most one multiplier. The peak hours from the
/// assignment (12–14) are inside the standard ones (9–18), so the standard zone is split in two:
/// 06–09 x0.90, 09–12 x1.00, 12–14 x1.15, 14–18 x1.00, 18–23 x0.80.
/// </para>
/// <para>
/// Time not covered by any zone (23:00–06:00 with the zones above) is outside working hours,
/// and a period that touches it can't be booked.
/// </para>
/// </summary>
public sealed class PricingPolicy
{
    private readonly IReadOnlyList<HoursPricing> _zones;

    /// <param name="zones">Tariff zones, loaded by the Application layer (from the database or configuration).</param>
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

        // A period may run past midnight, so zones are checked on every day the period touches.
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

        // Zones don't overlap, so the covered time equals the duration only when no minute is outside them.
        if (coveredTime != period.Duration)
            throw new DomainException("The hall can be booked only during working hours");

        return decimal.Round(rent, 2, MidpointRounding.AwayFromZero);
    }

    /// <summary>Sorted by start, so if any two zones overlap, two neighbouring ones do too.</summary>
    private static void EnsureNoOverlaps(IReadOnlyList<HoursPricing> sortedZones)
    {
        for (var i = 1; i < sortedZones.Count; i++)
        {
            if (sortedZones[i - 1].Overlaps(sortedZones[i]))
                throw new DomainException(
                    $"Hours pricing zones {sortedZones[i - 1].Name} and {sortedZones[i].Name} overlap");
        }
    }

    /// <summary>Ticks keep the value exact, unlike TotalHours (double): 10 minutes is 0.1666… h.</summary>
    private static decimal ToHours(TimeSpan time) => (decimal)time.Ticks / TimeSpan.TicksPerHour;

    private static DateTime Later(DateTime a, DateTime b) => a > b ? a : b;

    private static DateTime Earlier(DateTime a, DateTime b) => a < b ? a : b;
}
