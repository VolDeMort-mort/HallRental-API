using HallRental.Bll.Common.Pricing;
using HallRental.Bll.Common.Reports;
using HallRental.Bll.Common.Reports.Models;
using HallRental.Bll.Common.Shared.Exceptions;
using HallRental.Bll.Reports;
using HallRental.Bll.Reports.Validators;
using NSubstitute;

namespace HallRental.Bll.Tests.Reports;

public class ReportManagerTests
{
    private static readonly ReportPeriod September = new(new DateOnly(2030, 9, 1), new DateOnly(2030, 10, 1));

    private readonly IReportRepository _reports = Substitute.For<IReportRepository>();
    private readonly IPricingManager _pricing = Substitute.For<IPricingManager>();
    private readonly ReportManager _manager;

    public ReportManagerTests()
    {
        _pricing.GetWorkingHoursPerDayAsync(Arg.Any<CancellationToken>()).Returns(17m);
        _manager = new ReportManager(_reports, _pricing, new ReportPeriodValidator());
    }

    [Fact]
    public async Task GetHallsReportAsync_CalculatesHoursOccupancyAndTotals()
    {
        var busy = new HallReportRow(Guid.NewGuid(), "Зал A", true, 2, 510, 17000m, 800m);
        var idle = new HallReportRow(Guid.NewGuid(), "Зал B", false, 0, 0, 0m, 0m);
        _reports.GetHallRowsAsync(new DateTime(2030, 9, 1), new DateTime(2030, 10, 1), Arg.Any<CancellationToken>())
            .Returns([busy, idle]);

        var report = await _manager.GetHallsReportAsync(September, CancellationToken.None);

        // 510 minutes = 8.5 h out of 17 h × 30 days = 510 h
        var hall = report.Halls[0];
        Assert.Equal(8.5m, hall.BookedHours);
        Assert.Equal(1.67m, hall.OccupancyPercent);
        Assert.Equal(17800m, hall.TotalRevenue);

        Assert.Equal(new HallsReportTotal(2, 8.5m, 17000m, 800m, 17800m), report.Total);
    }

    [Fact]
    public async Task GetServicesReportAsync_SumsRevenue()
    {
        _reports.GetServiceRowsAsync(new DateTime(2030, 9, 1), new DateTime(2030, 10, 1), Arg.Any<CancellationToken>())
            .Returns([new ServiceReport("Wi-Fi", 3, 900m), new ServiceReport("Звук", 1, 700m)]);

        var report = await _manager.GetServicesReportAsync(September, CancellationToken.None);

        Assert.Equal(1600m, report.TotalRevenue);
    }

    [Theory]
    [InlineData("2030-09-01", "2030-09-01")]
    [InlineData("2030-01-01", "2031-01-03")]
    public async Task GetHallsReportAsync_InvalidPeriod_ThrowsValidationFailed(string from, string to)
    {
        var period = new ReportPeriod(DateOnly.Parse(from), DateOnly.Parse(to));

        await Assert.ThrowsAsync<ValidationFailedException>(() => _manager.GetHallsReportAsync(period, CancellationToken.None));
        await _reports.DidNotReceive().GetHallRowsAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>());
    }
}
