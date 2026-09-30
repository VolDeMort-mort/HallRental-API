using MediatR;
using HallRental.Application.Interfaces;
using HallRental.Application.Exceptions;
using HallRental.Domain.Entities;

namespace HallRental.Application.Halls.DeleteHall;

public class DeleteHallCommandHandler: IRequestHandler<DeleteHallCommand>
{
    private readonly IHallRepository _halls;
    private readonly IBookingRepository _bookings;
    private readonly TimeProvider _timeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteHallCommandHandler(IHallRepository halls, IBookingRepository bookings, TimeProvider timeProvider, IUnitOfWork unitOfWork)
    {
        _halls = halls;
        _bookings = bookings;
        _timeProvider = timeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteHallCommand request, CancellationToken cancellationToken)
    {
        Hall? hall = await _halls.GetByIdAsync(request.Id, cancellationToken);

        if (hall == null)
            throw new NotFoundException("Hall was not found");

        var now = _timeProvider.GetLocalNow().DateTime;
        var hasFutureBookings = await _bookings.HasFutureBookingsAsync(hall.Id, now, cancellationToken);

        if (hasFutureBookings)
            throw new ConflictException("The hall has upcoming bookings and can't be deleted");

        hall.Deactivate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
