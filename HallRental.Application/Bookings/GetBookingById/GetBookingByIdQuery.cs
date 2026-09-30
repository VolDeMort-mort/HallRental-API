using MediatR;

namespace HallRental.Application.Bookings.GetBookingById;

public record GetBookingByIdQuery(Guid Id) : IRequest<BookingDto>;
