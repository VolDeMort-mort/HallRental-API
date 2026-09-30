using HallRental.Domain.Entities;
using HallRental.Domain.ValueObjects;

namespace HallRental.Application.Interfaces;

/// <summary>
/// Halls are soft-deleted, so reads return only active halls: a deleted hall is "not found".
/// The one exception is <see cref="GetNameAsync"/>.
/// Halls are always loaded together with their services (the domain needs them for duplicate checks and bookings).
/// </summary>
public interface IHallRepository
{
    Task AddAsync(Hall hall, CancellationToken cancellationToken);

    Task<Hall?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>Active halls with at least <paramref name="minCapacity"/> seats and no booking overlapping the period, ordered by name.</summary>
    Task<IReadOnlyList<Hall>> GetAvailableAsync(int minCapacity, RentalPeriod rentalPeriod, CancellationToken cancellationToken);

    /// <summary>
    /// Name of a hall, deleted ones included: a booking keeps showing the hall it was made for.
    /// Bookings reference halls with a foreign key, so the hall always exists.
    /// </summary>
    Task<string> GetNameAsync(Guid id, CancellationToken cancellationToken);
}
