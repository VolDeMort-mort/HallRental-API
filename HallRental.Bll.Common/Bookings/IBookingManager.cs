using HallRental.Bll.Common.Bookings.Models;

namespace HallRental.Bll.Common.Bookings;

public interface IBookingManager
{
    Task<Booking> CreateAsync(NewBooking booking, CancellationToken cancellationToken);

    Task<Booking> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
