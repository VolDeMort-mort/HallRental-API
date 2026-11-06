using HallRental.Bll.Common.Pricing;
using HallRental.Utils;

namespace HallRental.Bll.Pricing;

public class PricingManager : IPricingManager
{
    private readonly IHoursPricingRepository _pricing;

    public PricingManager(IHoursPricingRepository pricing)
    {
        _pricing = pricing;
    }

    public async Task<decimal> CalculateRentAsync(decimal pricePerHour, DateTime start, DateTime end, CancellationToken cancellationToken)
    {
        var zones = await _pricing.GetAllAsync(cancellationToken);
        var pricingPolicy = new PricingPolicy(zones);

        return pricingPolicy.CalculateRent(pricePerHour, start, end);
    }

    public async Task<decimal> GetWorkingHoursPerDayAsync(CancellationToken cancellationToken)
    {
        var zones = await _pricing.GetAllAsync(cancellationToken);

        return zones.Sum(z => (z.To - z.From).ToHours());
    }
}
