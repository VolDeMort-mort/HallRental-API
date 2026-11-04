using AutoMapper;
using HallRental.Bll.Common.Halls;
using HallRental.Bll.Common.Halls.Models;
using HallRental.Dal.SqlRepositories.Halls.Entities;
using Microsoft.EntityFrameworkCore;

namespace HallRental.Dal.SqlRepositories.Halls;

public class SqlHallRepository : IHallRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public SqlHallRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task AddAsync(Hall hall, CancellationToken cancellationToken)
    {
        await _context.Halls.AddAsync(_mapper.Map<HallEntity>(hall), cancellationToken);
    }

    public async Task<Hall?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var hall = await _context.Halls
            .AsNoTracking()
            .Include(h => h.Services)
            .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);

        return hall == null ? null : _mapper.Map<Hall>(hall);
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

        return _mapper.Map<List<Hall>>(halls);
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
        await _context.Services.AddAsync(_mapper.Map<ServiceEntity>(service), cancellationToken);
    }

    // Includes bookings going on right now
    public Task<bool> HasFutureBookingsAsync(Guid hallId, DateTime now, CancellationToken cancellationToken) =>
        _context.Bookings.AnyAsync(b => b.HallId == hallId && b.End > now, cancellationToken);
}
