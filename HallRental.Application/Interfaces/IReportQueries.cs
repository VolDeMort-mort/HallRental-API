using HallRental.Application.Reports.HallsReport;
using HallRental.Application.Reports.ServicesReport;

namespace HallRental.Application.Interfaces;

// Read side for reports: the database groups and sums, only ready rows come back
public interface IReportQueries
{
    Task<IReadOnlyList<HallReportRow>> GetHallRowsAsync(DateTime from, DateTime to, CancellationToken cancellationToken);

    Task<IReadOnlyList<ServiceReportDto>> GetServiceRowsAsync(DateTime from, DateTime to, CancellationToken cancellationToken);
}
