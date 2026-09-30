using HallRental.Domain.Entities;
using HallRental.Domain.ValueObjects;

namespace HallRental.Application.Interfaces;

// Reads return only active halls (soft delete) together with their services; GetNameAsync is the exception
public interface IHallRepository
{
    Task AddAsync(Hall hall, CancellationToken cancellationToken);

    Task<Hall?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Hall>> GetAvailableAsync(int minCapacity, RentalPeriod rentalPeriod, CancellationToken cancellationToken);

    // Deleted halls included: a booking keeps showing the hall it was made for
    Task<string> GetNameAsync(Guid id, CancellationToken cancellationToken);
}
