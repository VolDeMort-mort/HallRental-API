namespace HallRental.Bll.Common.Pricing;

public interface IPricingManager
{
    Task<decimal> CalculateRentAsync(decimal pricePerHour, DateTime start, DateTime end, CancellationToken cancellationToken);

    Task<decimal> GetWorkingHoursPerDayAsync(CancellationToken cancellationToken);
}
