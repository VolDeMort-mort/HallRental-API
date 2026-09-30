using MediatR;
using HallRental.Application.Interfaces;
using HallRental.Application.Exceptions;
using HallRental.Domain.Entities;

namespace HallRental.Application.Bookings.GetBookingById;

public class GetBookingByIdQueryHandler : IRequestHandler<GetBookingByIdQuery, BookingDto>
{
    private readonly IBookingRepository _bookings;
    private readonly IHallRepository _halls;

    public GetBookingByIdQueryHandler(IBookingRepository bookings, IHallRepository halls)
    {
        _bookings = bookings;
        _halls = halls;
    }

    public async Task<BookingDto> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
    {
        Booking? booking = await _bookings.GetByIdAsync(request.Id, cancellationToken);

        if (booking == null)
            throw new NotFoundException("Booking was not found");

        // The hall may be deleted by now, but the booking still shows the hall it was made for
        var hallName = await _halls.GetNameAsync(booking.HallId, cancellationToken);

        return booking.ToDto(hallName);
    }
}
