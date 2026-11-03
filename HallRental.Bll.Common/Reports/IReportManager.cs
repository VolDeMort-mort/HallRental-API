using HallRental.Bll.Common.Reports.Models;

namespace HallRental.Bll.Common.Reports;

public interface IReportManager
{
    Task<HallsReport> GetHallsReportAsync(ReportPeriod period, CancellationToken cancellationToken);

    Task<ServicesReport> GetServicesReportAsync(ReportPeriod period, CancellationToken cancellationToken);
}
