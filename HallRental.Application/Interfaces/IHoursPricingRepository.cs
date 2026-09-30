using HallRental.Domain.Entities;

namespace HallRental.Application.Interfaces;

public interface IHoursPricingRepository
{
    Task<IReadOnlyList<HoursPricing>> GetAllAsync(CancellationToken cancellationToken);
}
