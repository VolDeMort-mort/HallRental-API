using MediatR;

namespace HallRental.Application.Bookings.CreateBooking;

/// <summary>Only the ids of services come from the client; their prices always come from the hall.</summary>
public record CreateBookingCommand(
    Guid HallId,
    DateTime Start,
    TimeSpan Duration,
    IReadOnlyList<Guid> ServiceIds
): IRequest<BookingDto>;
