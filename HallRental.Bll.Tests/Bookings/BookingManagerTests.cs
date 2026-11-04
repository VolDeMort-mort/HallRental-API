using HallRental.Bll.Bookings;
using HallRental.Bll.Bookings.Validators;
using HallRental.Bll.Common.Bookings;
using HallRental.Bll.Common.Bookings.Models;
using HallRental.Bll.Common.Halls;
using HallRental.Bll.Common.Halls.Models;
using HallRental.Bll.Common.Pricing;
using HallRental.Bll.Common.Shared;
using HallRental.Bll.Common.Shared.Exceptions;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;

namespace HallRental.Bll.Tests.Bookings;

public class BookingManagerTests
{
    private const string ClientId = "client-1";
    private static readonly DateTime Now = new(2030, 1, 1, 9, 0, 0);
    private static readonly DateTime Start = new(2030, 9, 2, 10, 0, 0);

    private readonly IHallManager _halls = Substitute.For<IHallManager>();
    private readonly IBookingRepository _bookings = Substitute.For<IBookingRepository>();
    private readonly IPricingManager _pricing = Substitute.For<IPricingManager>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly FakeTimeProvider _timeProvider = new();
    private readonly BookingManager _manager;

    private readonly Service _projector = new() { Id = Guid.NewGuid(), Name = "Проєктор", Price = 500m };
    private readonly Service _wifi = new() { Id = Guid.NewGuid(), Name = "Wi-Fi", Price = 300m };
    private readonly Hall _hall;

    public BookingManagerTests()
    {
        _timeProvider.SetLocalTimeZone(TimeZoneInfo.Utc);
        _timeProvider.SetUtcNow(new DateTimeOffset(Now, TimeSpan.Zero));
        _currentUser.Id.Returns(ClientId);

        _hall = new Hall
        {
            Id = Guid.NewGuid(),
            Name = "Зал A",
            Capacity = 50,
            PricePerHour = 2000m,
            IsActive = true,
            Services = [_projector, _wifi]
        };

        _halls.GetByIdAsync(_hall.Id, Arg.Any<CancellationToken>()).Returns(_hall);
        _pricing.CalculateRentAsync(Arg.Any<decimal>(), Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(8600m);

        _manager = new BookingManager(
            _halls, _bookings, _pricing, _timeProvider, _unitOfWork, _currentUser, new NewBookingValidator());
    }

    private NewBooking NewBooking(params Guid[] serviceIds) => new()
    {
        HallId = _hall.Id,
        Start = Start,
        Duration = TimeSpan.FromHours(4),
        ServiceIds = serviceIds.ToList()
    };

    [Fact]
    public async Task CreateAsync_ValidBooking_SavesBookingWithPricesAndServices()
    {
        var booking = await _manager.CreateAsync(NewBooking(_projector.Id, _wifi.Id), CancellationToken.None);

        Assert.Equal(ClientId, booking.ClientId);
        Assert.Equal(Start.AddHours(4), booking.End);
        Assert.Equal(8600m, booking.RentPrice);
        Assert.Equal(800m, booking.ServicesPrice);
        Assert.Equal(9400m, booking.TotalPrice);
        Assert.Equal([_projector.Id, _wifi.Id], booking.Services.Select(s => s.ServiceId));

        await _bookings.Received(1).AddAsync(booking, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_UnknownHall_ThrowsNotFound()
    {
        var newBooking = NewBooking();
        newBooking.HallId = Guid.NewGuid();

        _halls.GetByIdAsync(newBooking.HallId, Arg.Any<CancellationToken>())
        .Returns<Hall>(_ => throw new NotFoundException("Hall was not found"));

        await Assert.ThrowsAsync<NotFoundException>(() => _manager.CreateAsync(newBooking, CancellationToken.None));
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_HallIsBusy_ThrowsConflict()
    {
        _bookings.HasOverlapAsync(_hall.Id, Start, Start.AddHours(4), Arg.Any<CancellationToken>()).Returns(true);

        await Assert.ThrowsAsync<ConflictException>(() => _manager.CreateAsync(NewBooking(), CancellationToken.None));
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_StartInThePast_ThrowsBusinessRule()
    {
        _timeProvider.SetUtcNow(new DateTimeOffset(Start.AddMinutes(1), TimeSpan.Zero));

        var exception = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _manager.CreateAsync(NewBooking(), CancellationToken.None));

        Assert.Equal("A booking can't start in the past", exception.Message);
    }

    [Fact]
    public async Task GetByIdAsync_SomeoneElsesBooking_ThrowsNotFound()
    {
        var booking = ExistingBooking("client-2");

        await Assert.ThrowsAsync<NotFoundException>(() => _manager.GetByIdAsync(booking.Id, CancellationToken.None));
    }

    [Fact]
    public async Task GetByIdAsync_AdminSeesAnyBooking()
    {
        _currentUser.IsAdmin.Returns(true);
        var booking = ExistingBooking("client-2");

        var result = await _manager.GetByIdAsync(booking.Id, CancellationToken.None);

        Assert.Same(booking, result);
    }

    private Booking ExistingBooking(string clientId)
    {
        var booking = new Booking { Id = Guid.NewGuid(), HallId = _hall.Id, ClientId = clientId };
        _bookings.GetByIdAsync(booking.Id, Arg.Any<CancellationToken>()).Returns(booking);
        return booking;
    }
}
