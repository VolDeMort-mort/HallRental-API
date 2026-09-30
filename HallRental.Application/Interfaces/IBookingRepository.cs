using HallRental.Domain.Entities;
using HallRental.Domain.ValueObjects;

namespace HallRental.Application.Interfaces;

public interface IBookingRepository
{
    Task AddAsync(Booking booking, CancellationToken cancellationToken);

    Task<bool> HasOverlapAsync(Guid hallId, RentalPeriod period, CancellationToken cancellationToken);

    /// <summary>Bookings that haven't ended yet, including the ones going on right now.</summary>
    Task<bool> HasFutureBookingsAsync(Guid hallId, DateTime now, CancellationToken cancellationToken);
}
