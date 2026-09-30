using MediatR;

namespace HallRental.Application.Bookings.CreateBooking;

/// <param name="HallId">The hall to book.</param>
/// <param name="Start">Start of the rent, hall's local time.</param>
/// <param name="Duration">Length of the rent.</param>
/// <param name="ServiceIds">Services of the hall the client picks; prices always come from the hall, never from the client.</param>
public record CreateBookingCommand(
    Guid HallId,
    DateTime Start,
    TimeSpan Duration,
    IReadOnlyList<Guid> ServiceIds
): IRequest<BookingDto>;
