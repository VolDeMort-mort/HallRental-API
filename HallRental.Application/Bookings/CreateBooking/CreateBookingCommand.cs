using MediatR;

namespace HallRental.Application.Bookings.CreateBooking;

/// <param name="ServiceIds">Services of the hall the client picks; prices always come from the hall, never from the client.</param>
public record CreateBookingCommand(
    Guid HallId,
    DateTime Start,
    TimeSpan Duration,
    IReadOnlyList<Guid> ServiceIds
): IRequest<BookingDto>;
