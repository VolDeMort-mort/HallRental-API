using HallRental.Application.Interfaces;
using HallRental.Domain.Entities;
using HallRental.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace HallRental.Infrastructure.Persistence.Repositories;

public class HallRepository: IHallRepository
{
    private readonly AppDbContext _context;
    public HallRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Hall hall, CancellationToken cancellationToken)
    {
        await _context.Halls.AddAsync(hall, cancellationToken);
    }

    // Tracked: commands change the hall, and SaveChanges has to see it
    public async Task<Hall?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Halls
            .Include(h => h.Services)
            .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Hall>> GetAvailableAsync(int minCapacity, RentalPeriod rentalPeriod, CancellationToken cancellationToken)
    {
        return await _context.Halls
            .AsNoTracking()
            .Include(h => h.Services)
            .Where(h => h.Capacity >= minCapacity
                     && !_context.Bookings.Any(b => b.HallId == h.Id
                                                 && b.Period.Start < rentalPeriod.End
                                                 && rentalPeriod.Start < b.Period.End))
            .OrderBy(h => h.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<string> GetNameAsync(Guid id, CancellationToken cancellationToken) =>
        _context.Halls
            .IgnoreQueryFilters()
            .Where(h => h.Id == id)
            .Select(h => h.Name)
            .FirstAsync(cancellationToken);
}
