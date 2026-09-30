using HallRental.Application.Interfaces;
using HallRental.Domain.Entities;
using HallRental.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace HallRental.Infrastructure.Persistence.Repositories;

public class BookingRepository: IBookingRepository
{
    private readonly AppDbContext _context;
    public BookingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Booking booking, CancellationToken cancellationToken) => await _context.Bookings.AddAsync(booking, cancellationToken);

    public Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        _context.Bookings.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

    // Same formula as RentalPeriod.Overlaps, translated to SQL
    public Task<bool> HasOverlapAsync(Guid hallId, RentalPeriod period, CancellationToken cancellationToken) =>
        _context.Bookings.AnyAsync(b => b.HallId == hallId
                                     && b.Period.Start < period.End
                                     && period.Start < b.Period.End, cancellationToken);

    public Task<bool> HasFutureBookingsAsync(Guid hallId, DateTime now, CancellationToken cancellationToken) =>
        _context.Bookings.AnyAsync(b => b.HallId == hallId && b.Period.End > now, cancellationToken);
}
