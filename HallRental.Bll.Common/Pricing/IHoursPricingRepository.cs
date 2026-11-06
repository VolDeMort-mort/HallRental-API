using HallRental.Bll.Common.Pricing.Models;

namespace HallRental.Bll.Common.Pricing;

public interface IHoursPricingRepository
{
    Task<IReadOnlyList<HoursPricing>> GetAllAsync(CancellationToken cancellationToken);
}
