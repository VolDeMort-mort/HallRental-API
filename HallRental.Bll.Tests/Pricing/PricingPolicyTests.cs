using HallRental.Bll.Common.Pricing.Exceptions;
using HallRental.Bll.Pricing;
using HallRental.Bll.Tests.Shared;

namespace HallRental.Bll.Tests.Pricing;

public class PricingPolicyTests
{
    private static readonly DateTime Day = new(2030, 9, 2);

    private readonly PricingPolicy _policy = new(TestZones.Default());

    [Theory]
    [InlineData(10, 11, 1000)]
    [InlineData(11, 13, 2150)]
    [InlineData(6, 23, 16000)]
    public void CalculateRent_WithinWorkingHours_AppliesZoneMultipliers(int fromHour, int toHour, decimal expected)
    {
        var rent = _policy.CalculateRent(1000m, Day.AddHours(fromHour), Day.AddHours(toHour));

        Assert.Equal(expected, rent);
    }

    [Fact]
    public void CalculateRent_PartOfHour_RoundsToCents()
    {
        var rent = _policy.CalculateRent(1000m, Day.AddHours(10), Day.AddHours(10).AddMinutes(20));

        Assert.Equal(333.33m, rent);
    }

    [Theory]
    [InlineData(5, 7)]
    [InlineData(22, 25)]
    public void CalculateRent_OutsideWorkingHours_Throws(int fromHour, int toHour)
    {
        Assert.Throws<OutsideWorkingHoursException>(
            () => _policy.CalculateRent(1000m, Day.AddHours(fromHour), Day.AddHours(toHour)));
    }

    [Fact]
    public void Constructor_OverlappingZones_Throws()
    {
        var zones = new[]
        {
            TestZones.Zone("Standard", 9, 18, 1.00m),
            TestZones.Zone("Peak", 12, 14, 1.15m)
        };

        Assert.Throws<InvalidPricingZonesException>(() => new PricingPolicy(zones));
    }
}
