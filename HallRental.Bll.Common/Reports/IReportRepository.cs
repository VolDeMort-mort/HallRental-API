using HallRental.Bll.Common.Reports.Models;

namespace HallRental.Bll.Common.Reports;

public interface IReportRepository
{
    Task<IReadOnlyList<HallReportRow>> GetHallRowsAsync(DateTime from, DateTime to, CancellationToken cancellationToken);

    Task<IReadOnlyList<ServiceReport>> GetServiceRowsAsync(DateTime from, DateTime to, CancellationToken cancellationToken);
}
