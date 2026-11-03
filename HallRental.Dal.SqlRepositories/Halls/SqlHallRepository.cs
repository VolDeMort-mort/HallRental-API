using HallRental.Bll.Common.Halls;
using HallRental.Bll.Common.Halls.Models;
using HallRental.Dal.SqlRepositories.Mapping;
using Microsoft.EntityFrameworkCore;

namespace HallRental.Dal.SqlRepositories.Halls;

public class SqlHallRepository : IHallRepository
{
    private readonly AppDbContext _context;

    public SqlHallRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Hall hall, CancellationToken cancellationToken)
    {
        await _context.Halls.AddAsync(hall.ToEntity(), cancellationToken);
    }

    public async Task<Hall?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var hall = await _context.Halls
            .AsNoTracking()
            .Include(h => h.Services)
            .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);

        return hall?.ToModel();
    }

    public async Task<IReadOnlyList<Hall>> GetAvailableAsync(int minCapacity, DateTime start, DateTime end, CancellationToken cancellationToken)
    {
        var halls = await _context.Halls
            .AsNoTracking()
            .Include(h => h.Services)
            .Where(h => h.Capacity >= minCapacity
                     && !_context.Bookings.Any(b => b.HallId == h.Id
                                                 && b.Start < end
                                                 && start < b.End))
            .OrderBy(h => h.Name)
            .ToListAsync(cancellationToken);

        return halls.Select(h => h.ToModel()).ToList();
    }

    public async Task UpdateAsync(Hall hall, CancellationToken cancellationToken)
    {
        var entity = await _context.Halls.FirstAsync(h => h.Id == hall.Id, cancellationToken);

        entity.Name = hall.Name;
        entity.Capacity = hall.Capacity;
        entity.PricePerHour = hall.PricePerHour;
        entity.IsActive = hall.IsActive;
    }

    public async Task AddServiceAsync(Service service, CancellationToken cancellationToken)
    {
        await _context.Services.AddAsync(service.ToEntity(), cancellationToken);
    }

    // Includes bookings going on right now
    public Task<bool> HasFutureBookingsAsync(Guid hallId, DateTime now, CancellationToken cancellationToken) =>
        _context.Bookings.AnyAsync(b => b.HallId == hallId && b.End > now, cancellationToken);
}
