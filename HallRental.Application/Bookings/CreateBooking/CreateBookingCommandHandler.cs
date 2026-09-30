using MediatR;
using HallRental.Application.Interfaces;
using HallRental.Application.Exceptions;
using HallRental.Domain.Entities;
using HallRental.Domain.Pricing;
using HallRental.Domain.ValueObjects;

namespace HallRental.Application.Bookings.CreateBooking;

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, BookingDto>
{
    private readonly IHallRepository _halls;
    private readonly IBookingRepository _bookings;
    private readonly IHoursPricingRepository _pricing;
    private readonly TimeProvider _timeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBookingCommandHandler(IHallRepository halls, IUnitOfWork unitOfWork, IBookingRepository bookings, IHoursPricingRepository pricing, TimeProvider timeProvider)
    {
        _halls = halls;
        _unitOfWork = unitOfWork;
        _bookings = bookings;
        _pricing = pricing;
        _timeProvider = timeProvider;
    }

    public async Task<BookingDto> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        var rentalPeriod = RentalPeriod.FromDuration(request.Start, request.Duration);
        var hall = await _halls.GetByIdAsync(request.HallId, cancellationToken);

        if (hall == null)
            throw new NotFoundException("Hall was not found");

        var isBusy = await _bookings.HasOverlapAsync(hall.Id, rentalPeriod, cancellationToken);

        if (isBusy)
            throw new ConflictException("The hall is already booked at this time");

        // Tariffs are read only after the cheap checks: a request for a missing or busy hall doesn't need them
        var zones = await _pricing.GetAllAsync(cancellationToken);
        var pricingPolicy = new PricingPolicy(zones);

        // Server's local time: if the server runs in UTC (Docker, cloud), the halls' time zone has to be set explicitly
        var now = _timeProvider.GetLocalNow().DateTime;

        var booking = Booking.Create(hall, rentalPeriod, request.ServiceIds, pricingPolicy, now);

        await _bookings.AddAsync(booking, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return booking.ToDto(hall.Name);
    }
}
