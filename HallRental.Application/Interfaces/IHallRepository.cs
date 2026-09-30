using HallRental.Domain.Entities;
using HallRental.Domain.ValueObjects;

namespace HallRental.Application.Interfaces;

/// <summary>
/// Halls are soft-deleted, so every read returns only active halls: a deleted hall is "not found".
/// Halls are always loaded together with their services (the domain needs them for duplicate checks and bookings).
/// </summary>
public interface IHallRepository
{
    Task AddAsync(Hall hall, CancellationToken cancellationToken);

    Task<Hall?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>Active halls with at least <paramref name="minCapacity"/> seats and no booking overlapping the period.</summary>
    Task<IReadOnlyList<Hall>> GetAvailableAsync(int minCapacity, RentalPeriod rentalPeriod, CancellationToken cancellationToken);
}
