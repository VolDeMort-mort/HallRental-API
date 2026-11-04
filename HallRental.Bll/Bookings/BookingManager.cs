using FluentValidation;
using HallRental.Bll.Common.Bookings;
using HallRental.Bll.Common.Bookings.Models;
using HallRental.Bll.Common.Halls;
using HallRental.Bll.Common.Halls.Models;
using HallRental.Bll.Common.Pricing;
using HallRental.Bll.Common.Shared;
using HallRental.Bll.Common.Shared.Constants;
using HallRental.Bll.Common.Shared.Exceptions;
using HallRental.Bll.Shared;
using HallRental.Bll.Common.Bookings.Exceptions;

namespace HallRental.Bll.Bookings;

public class BookingManager : IBookingManager
{
    private readonly IHallManager _halls;
    private readonly IBookingRepository _bookings;
    private readonly IPricingManager _pricing;
    private readonly TimeProvider _timeProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly IValidator<NewBooking> _validator;

    public BookingManager(
        IHallManager halls,
        IBookingRepository bookings,
        IPricingManager pricing,
        TimeProvider timeProvider,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        IValidator<NewBooking> validator)
    {
        _halls = halls;
        _bookings = bookings;
        _pricing = pricing;
        _timeProvider = timeProvider;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _validator = validator;
    }

    public async Task<Booking> CreateAsync(NewBooking newBooking, CancellationToken cancellationToken)
    {
        await _validator.EnsureValidAsync(newBooking, cancellationToken);

        var start = newBooking.Start;
        var end = start + newBooking.Duration;
        var hall = await _halls.GetByIdAsync(newBooking.HallId, cancellationToken);

        var isBusy = await _bookings.HasOverlapAsync(hall.Id, start, end, cancellationToken);

        if (isBusy)
            throw new ConflictException("The hall is already booked at this time");

        // Server's local time: on a UTC server (Docker, cloud) the halls' time zone has to be set explicitly
        var now = _timeProvider.GetLocalNow().DateTime;

        EnsureCanBook(_currentUser.Id, hall, start, now);

        var rentPrice = await _pricing.CalculateRentAsync(hall.PricePerHour, start, end, cancellationToken);
        var booking = CreateBooking(_currentUser.Id, hall, start, end, rentPrice, newBooking.ServiceIds);

        await _bookings.AddAsync(booking, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return booking;
    }

    public async Task<Booking> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        Booking? booking = await _bookings.GetByIdAsync(id, cancellationToken);

        // Someone else's booking looks like a missing one, so its existence isn't revealed
        if (booking == null || (booking.ClientId != _currentUser.Id && !_currentUser.IsAdmin))
            throw new NotFoundException("Booking was not found");

        return booking;
    }

    private static void EnsureCanBook(string clientId, Hall hall, DateTime start, DateTime now)
    {
        if (string.IsNullOrWhiteSpace(clientId) || clientId.Length > EntityLimits.ClientIdMaxLength)
            throw new InvalidClientException();

        if (!hall.IsActive)
            throw new HallNotActiveException();

        if (start < now)
            throw new BookingInPastException();
    }

    private static Booking CreateBooking(
        string clientId,
        Hall hall,
        DateTime start,
        DateTime end,
        decimal rentPrice,
        IReadOnlyCollection<Guid> serviceIds)
    {
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            HallId = hall.Id,
            HallName = hall.Name,
            ClientId = clientId,
            Start = start,
            End = end,
            RentPrice = rentPrice
        };

        // A copy taken at the booking moment, so later price changes don't affect existing bookings
        booking.Services = SelectServices(hall, serviceIds)
            .Select(s => new BookedService { ServiceId = s.Id, Name = s.Name, Price = s.Price })
            .ToList();
        booking.ServicesPrice = booking.Services.Sum(s => s.Price);

        return booking;
    }

    private static IReadOnlyList<Service> SelectServices(Hall hall, IReadOnlyCollection<Guid> serviceIds)
    {
        var requestedIds = serviceIds.Distinct().ToList();
        var selected = hall.Services.Where(s => requestedIds.Contains(s.Id)).ToList();

        if (selected.Count != requestedIds.Count)
            throw new ServiceNotAvailableException();

        return selected;
    }
}
