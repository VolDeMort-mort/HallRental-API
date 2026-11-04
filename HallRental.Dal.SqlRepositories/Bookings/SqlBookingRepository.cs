using AutoMapper;
using HallRental.Bll.Common.Bookings;
using HallRental.Bll.Common.Bookings.Models;
using HallRental.Dal.SqlRepositories.Bookings.Entities;
using Microsoft.EntityFrameworkCore;

namespace HallRental.Dal.SqlRepositories.Bookings;

public class SqlBookingRepository : IBookingRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public SqlBookingRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task AddAsync(Booking booking, CancellationToken cancellationToken)
    {
        await _context.Bookings.AddAsync(_mapper.Map<BookingEntity>(booking), cancellationToken);
    }

    public async Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

        if (booking == null)
            return null;

        // Deleted halls included: a booking keeps showing the hall it was made for
        var hallName = await _context.Halls
            .IgnoreQueryFilters()
            .Where(h => h.Id == booking.HallId)
            .Select(h => h.Name)
            .FirstAsync(cancellationToken);

        var model = _mapper.Map<Booking>(booking);
        model.HallName = hallName;

        return model;
    }

    // Same formula as the [Start, End) overlap: 10:00–12:00 and 12:00–14:00 don't overlap
    public Task<bool> HasOverlapAsync(Guid hallId, DateTime start, DateTime end, CancellationToken cancellationToken) =>
        _context.Bookings.AnyAsync(b => b.HallId == hallId
                                     && b.Start < end
                                     && start < b.End, cancellationToken);
}
