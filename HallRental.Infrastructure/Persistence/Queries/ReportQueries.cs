using HallRental.Application.Interfaces;
using HallRental.Application.Reports.HallsReport;
using HallRental.Application.Reports.ServicesReport;
using HallRental.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HallRental.Infrastructure.Persistence.Queries;

/// <summary>
/// Report queries: EF turns the GroupBy / Count / Sum below into one SQL statement per report,
/// so only the ready rows come back from the database.
/// </summary>
public class ReportQueries : IReportQueries
{
    private readonly AppDbContext _context;

    public ReportQueries(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<HallReportRow>> GetHallRowsAsync(DateTime from, DateTime to, CancellationToken cancellationToken)
    {
        var bookings = BookingsStartingIn(from, to);

        // Built from the halls, not from the bookings: an idle hall is an answer too and shows up with zeros.
        // TotalPrice isn't stored (it's calculated), so the sums use RentPrice and ServicesPrice.
        return await _context.Halls
            .IgnoreQueryFilters()
            .Where(h => h.IsActive || bookings.Any(b => b.HallId == h.Id))
            .OrderBy(h => h.Name)
            .Select(h => new HallReportRow(
                h.Id,
                h.Name,
                h.IsActive,
                bookings.Count(b => b.HallId == h.Id),
                bookings.Where(b => b.HallId == h.Id).Sum(b => EF.Functions.DateDiffMinute(b.Period.Start, b.Period.End)),
                bookings.Where(b => b.HallId == h.Id).Sum(b => b.RentPrice),
                bookings.Where(b => b.HallId == h.Id).Sum(b => b.ServicesPrice)))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ServiceReportDto>> GetServiceRowsAsync(DateTime from, DateTime to, CancellationToken cancellationToken)
    {
        // Grouped by the copies stored in the bookings: what was really sold, at the prices it was sold for
        return await BookingsStartingIn(from, to)
            .SelectMany(b => b.Services)
            .GroupBy(s => s.Name)
            .OrderByDescending(g => g.Count())
            .ThenBy(g => g.Key)
            .Select(g => new ServiceReportDto(g.Key, g.Count(), g.Sum(s => s.Price)))
            .ToListAsync(cancellationToken);
    }

    private IQueryable<Booking> BookingsStartingIn(DateTime from, DateTime to) =>
        _context.Bookings.Where(b => b.Period.Start >= from && b.Period.Start < to);
}
