using HallRental.Bll.Common.Pricing.Models;
using HallRental.Bll.Common.Pricing.Exceptions;
using HallRental.Utils;

namespace HallRental.Bll.Pricing;

// Zones must not overlap, so the standard 9–18 is split around the peak 12–14.
// Time outside every zone is outside working hours and can't be booked.
internal sealed class PricingPolicy
{
    private readonly IReadOnlyList<HoursPricing> _zones;

    public PricingPolicy(IEnumerable<HoursPricing> zones)
    {
        _zones = zones.OrderBy(z => z.From).ToList();

        if (_zones.Count == 0)
            throw new InvalidPricingZonesException("At least one hours pricing zone is required");

        EnsureNoOverlaps(_zones);
    }

    public decimal CalculateRent(decimal pricePerHour, DateTime periodStart, DateTime periodEnd)
    {
        decimal rent = 0;
        var coveredTime = TimeSpan.Zero;

        // A period may run past midnight, so zones are checked on every day it touches
        for (var day = periodStart.Date; day < periodEnd; day = day.AddDays(1))
        {
            foreach (var zone in _zones)
            {
                var start = periodStart.Later(day + zone.From.ToTimeSpan());
                var end = periodEnd.Earlier(day + zone.To.ToTimeSpan());
                if (start >= end)
                    continue;

                var timeInZone = end - start;
                rent += timeInZone.ToHours() * pricePerHour * zone.Multiplier;
                coveredTime += timeInZone;
            }
        }

        // Zones don't overlap, so this holds only when no minute is outside them
        if (coveredTime != periodEnd - periodStart)
            throw new OutsideWorkingHoursException();

        return decimal.Round(rent, 2, MidpointRounding.AwayFromZero);
    }

    // Sorted by start: if any two zones overlap, two neighbouring ones do too
    private static void EnsureNoOverlaps(IReadOnlyList<HoursPricing> sortedZones)
    {
        for (var i = 1; i < sortedZones.Count; i++)
        {
            var previous = sortedZones[i - 1];
            var current = sortedZones[i];

            if (previous.From < current.To && current.From < previous.To)
                throw new InvalidPricingZonesException(
                    $"Hours pricing zones {previous.Name} and {current.Name} overlap");
        }
    }
}
