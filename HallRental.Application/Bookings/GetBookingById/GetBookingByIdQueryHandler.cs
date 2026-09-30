using MediatR;
using HallRental.Application.Interfaces;
using HallRental.Application.Exceptions;
using HallRental.Domain.Entities;

namespace HallRental.Application.Bookings.GetBookingById;

public class GetBookingByIdQueryHandler : IRequestHandler<GetBookingByIdQuery, BookingDto>
{
    private readonly IBookingRepository _bookings;
    private readonly IHallRepository _halls;
    private readonly ICurrentUser _currentUser;

    public GetBookingByIdQueryHandler(IBookingRepository bookings, IHallRepository halls, ICurrentUser currentUser)
    {
        _bookings = bookings;
        _halls = halls;
        _currentUser = currentUser;
    }

    public async Task<BookingDto> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
    {
        Booking? booking = await _bookings.GetByIdAsync(request.Id, cancellationToken);

        // Someone else's booking looks like a missing one, so its existence isn't revealed
        if (booking == null || (booking.ClientId != _currentUser.Id && !_currentUser.IsAdmin))
            throw new NotFoundException("Booking was not found");

        var hallName = await _halls.GetNameAsync(booking.HallId, cancellationToken);

        return booking.ToDto(hallName);
    }
}
