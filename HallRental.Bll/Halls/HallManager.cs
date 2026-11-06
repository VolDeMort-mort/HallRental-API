using FluentValidation;
using HallRental.Bll.Common.Halls;
using HallRental.Bll.Common.Halls.Exceptions;
using HallRental.Bll.Common.Halls.Models;
using HallRental.Bll.Common.Shared;
using HallRental.Bll.Common.Shared.Exceptions;
using HallRental.Bll.Halls.Validators;
using HallRental.Bll.Shared;

namespace HallRental.Bll.Halls;

public class HallManager : IHallManager
{
    private readonly IHallRepository _halls;
    private readonly TimeProvider _timeProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<Hall> _hallValidator;
    private readonly IValidator<Service> _serviceValidator;
    private readonly IValidator<AvailableHallsFilter> _filterValidator;

    public HallManager(
        IHallRepository halls,
        TimeProvider timeProvider,
        IUnitOfWork unitOfWork,
        IValidator<Hall> hallValidator,
        IValidator<Service> serviceValidator,
        IValidator<AvailableHallsFilter> filterValidator)
    {
        _halls = halls;
        _timeProvider = timeProvider;
        _unitOfWork = unitOfWork;
        _hallValidator = hallValidator;
        _serviceValidator = serviceValidator;
        _filterValidator = filterValidator;
    }

    public async Task<Guid> CreateAsync(Hall hall, CancellationToken cancellationToken)
    {
        await _hallValidator.EnsureValidAsync(hall, cancellationToken);

        var newHall = new Hall
        {
            Id = Guid.NewGuid(),
            Name = hall.Name.Trim(),
            Capacity = hall.Capacity,
            PricePerHour = hall.PricePerHour,
            IsActive = true
        };

        foreach (var service in hall.Services)
        {
            AddService(newHall, service);
        }

        await _halls.AddAsync(newHall, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return newHall.Id;
    }

    public async Task<Hall> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        Hall? hall = await _halls.GetByIdAsync(id, cancellationToken);

        if (hall == null)
            throw new NotFoundException("Hall was not found");

        return hall;
    }

    public async Task UpdateAsync(Hall hall, CancellationToken cancellationToken)
    {
        await _hallValidator.EnsureValidAsync(hall, cancellationToken);

        var existing = await GetByIdAsync(hall.Id, cancellationToken);

        existing.Name = hall.Name.Trim();
        existing.Capacity = hall.Capacity;
        existing.PricePerHour = hall.PricePerHour;

        await _halls.UpdateAsync(existing, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Guid> AddServiceAsync(Guid hallId, Service service, CancellationToken cancellationToken)
    {
        await _serviceValidator.EnsureValidAsync(service, cancellationToken);

        var hall = await GetByIdAsync(hallId, cancellationToken);
        var newService = AddService(hall, service);

        await _halls.AddServiceAsync(newService, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return newService.Id;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var hall = await GetByIdAsync(id, cancellationToken);

        var now = _timeProvider.GetLocalNow().DateTime;
        var hasFutureBookings = await _halls.HasFutureBookingsAsync(hall.Id, now, cancellationToken);

        if (hasFutureBookings)
            throw new ConflictException("The hall has upcoming bookings and can't be deleted");

        // Soft delete: past bookings stay for history and reports
        hall.IsActive = false;

        await _halls.UpdateAsync(hall, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Hall>> GetAvailableAsync(DateTime start, DateTime end, int minCapacity, CancellationToken cancellationToken)
    {
        await _filterValidator.EnsureValidAsync(new AvailableHallsFilter(start, end, minCapacity), cancellationToken);

        return await _halls.GetAvailableAsync(minCapacity, start, end, cancellationToken);
    }

    private static Service AddService(Hall hall, Service service)
    {
        var name = service.Name.Trim();

        if (hall.Services.Any(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase)))
            throw new DuplicateServiceException(name);

        var newService = new Service
        {
            Id = Guid.NewGuid(),
            HallId = hall.Id,
            Name = name,
            Price = service.Price
        };

        hall.Services.Add(newService);
        return newService;
    }
}
