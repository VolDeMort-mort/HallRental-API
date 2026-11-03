using HallRental.Bll.Common.Bookings.Models;

namespace HallRental.Bll.Common.Bookings;

public interface IBookingRepository
{
    Task AddAsync(Booking booking, CancellationToken cancellationToken);

    Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> HasOverlapAsync(Guid hallId, DateTime start, DateTime end, CancellationToken cancellationToken);
}
