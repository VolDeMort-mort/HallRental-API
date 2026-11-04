using HallRental.Bll.Common.Halls;
using HallRental.Bll.Common.Halls.Models;
using HallRental.Bll.Common.Shared;
using HallRental.Bll.Common.Shared.Exceptions;
using HallRental.Bll.Halls;
using HallRental.Bll.Halls.Validators;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;

namespace HallRental.Bll.Tests.Halls;

public class HallManagerTests
{
    private static readonly DateTime Now = new(2030, 1, 1, 9, 0, 0);

    private readonly IHallRepository _halls = Substitute.For<IHallRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly FakeTimeProvider _timeProvider = new();
    private readonly HallManager _manager;

    public HallManagerTests()
    {
        _timeProvider.SetLocalTimeZone(TimeZoneInfo.Utc);
        _timeProvider.SetUtcNow(new DateTimeOffset(Now, TimeSpan.Zero));

        _manager = new HallManager(_halls, _timeProvider, _unitOfWork, new HallValidator(), new ServiceValidator(), new AvailableHallsFilterValidator());
    }

    private static Hall NewHall(params Service[] services) => new()
    {
        Name = "Зал D",
        Capacity = 40,
        PricePerHour = 1800m,
        Services = services.ToList()
    };

    private static Service NewService(string name, decimal price = 500m) => new() { Name = name, Price = price };

    private Hall ExistingHall()
    {
        var hall = NewHall();
        hall.Id = Guid.NewGuid();
        hall.IsActive = true;
        _halls.GetByIdAsync(hall.Id, Arg.Any<CancellationToken>()).Returns(hall);
        return hall;
    }

    [Fact]
    public async Task CreateAsync_ValidHall_SavesTrimmedActiveHallWithServices()
    {
        Hall? saved = null;
        await _halls.AddAsync(Arg.Do<Hall>(h => saved = h), Arg.Any<CancellationToken>());
        var hall = NewHall(NewService(" Проєктор "), NewService("Wi-Fi", 300m));
        hall.Name = "  Зал D  ";

        var id = await _manager.CreateAsync(hall, CancellationToken.None);

        Assert.NotNull(saved);
        Assert.Equal(id, saved.Id);
        Assert.Equal("Зал D", saved.Name);
        Assert.True(saved.IsActive);
        Assert.Equal(["Проєктор", "Wi-Fi"], saved.Services.Select(s => s.Name));
        Assert.All(saved.Services, s => Assert.Equal(id, s.HallId));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_DuplicateServiceInAnyCase_ThrowsBusinessRule()
    {
        var hall = NewHall(NewService("Звук"), NewService(" звук "));

        await Assert.ThrowsAsync<BusinessRuleException>(() => _manager.CreateAsync(hall, CancellationToken.None));
        await _halls.DidNotReceive().AddAsync(Arg.Any<Hall>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_InvalidHall_ThrowsValidationFailedWithAllErrors()
    {
        var hall = new Hall { Name = "", Capacity = 0, PricePerHour = -1m, Services = [NewService("", 0m)] };

        var exception = await Assert.ThrowsAsync<ValidationFailedException>(
            () => _manager.CreateAsync(hall, CancellationToken.None));

        Assert.Equal(
            ["Capacity", "Name", "PricePerHour", "Services[0].Name", "Services[0].Price"],
            exception.Errors.Keys.Order());
    }

    [Fact]
    public async Task UpdateAsync_ExistingHall_ChangesOnlyEditableFields()
    {
        var existing = ExistingHall();
        var changes = new Hall { Id = existing.Id, Name = " Зал E ", Capacity = 60, PricePerHour = 2500m };

        await _manager.UpdateAsync(changes, CancellationToken.None);

        Assert.Equal("Зал E", existing.Name);
        Assert.Equal(60, existing.Capacity);
        Assert.Equal(2500m, existing.PricePerHour);
        Assert.True(existing.IsActive);
        await _halls.Received(1).UpdateAsync(existing, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_NoFutureBookings_DeactivatesHall()
    {
        var hall = ExistingHall();

        await _manager.DeleteAsync(hall.Id, CancellationToken.None);

        Assert.False(hall.IsActive);
        await _halls.Received(1).HasFutureBookingsAsync(hall.Id, Now, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_FutureBookings_ThrowsConflict()
    {
        var hall = ExistingHall();
        _halls.HasFutureBookingsAsync(hall.Id, Arg.Any<DateTime>(), Arg.Any<CancellationToken>()).Returns(true);

        await Assert.ThrowsAsync<ConflictException>(() => _manager.DeleteAsync(hall.Id, CancellationToken.None));

        Assert.True(hall.IsActive);
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
