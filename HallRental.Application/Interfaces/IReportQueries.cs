using HallRental.Application.Reports.HallsReport;
using HallRental.Application.Reports.ServicesReport;

namespace HallRental.Application.Interfaces;

/// <summary>
/// Read side for reports: the database groups and sums the bookings and returns ready rows,
/// instead of loading every booking into memory through the repositories.
/// A booking belongs to the period [from, to) by its start.
/// </summary>
public interface IReportQueries
{
    /// <summary>Every active hall (with zeros if it has no bookings) and every deleted hall with bookings in the period, ordered by name.</summary>
    Task<IReadOnlyList<HallReportRow>> GetHallRowsAsync(DateTime from, DateTime to, CancellationToken cancellationToken);

    /// <summary>Booked services grouped by name, the most booked first.</summary>
    Task<IReadOnlyList<ServiceReportDto>> GetServiceRowsAsync(DateTime from, DateTime to, CancellationToken cancellationToken);
}
