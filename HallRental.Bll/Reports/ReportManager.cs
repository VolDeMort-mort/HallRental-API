using FluentValidation;
using HallRental.Bll.Common.Pricing;
using HallRental.Bll.Common.Reports;
using HallRental.Bll.Common.Reports.Models;
using HallRental.Bll.Shared;

namespace HallRental.Bll.Reports;

public class ReportManager : IReportManager
{
    private readonly IReportRepository _reports;
    private readonly IPricingManager _pricing;
    private readonly IValidator<ReportPeriod> _validator;

    public ReportManager(IReportRepository reports, IPricingManager pricing, IValidator<ReportPeriod> validator)
    {
        _reports = reports;
        _pricing = pricing;
        _validator = validator;
    }

    public async Task<HallsReport> GetHallsReportAsync(ReportPeriod period, CancellationToken cancellationToken)
    {
        await _validator.EnsureValidAsync(period, cancellationToken);

        var from = period.From.ToDateTime(TimeOnly.MinValue);
        var to = period.To.ToDateTime(TimeOnly.MinValue);

        var rows = await _reports.GetHallRowsAsync(from, to, cancellationToken);

        var workingHoursPerDay = await _pricing.GetWorkingHoursPerDayAsync(cancellationToken);
        var availableHours = workingHoursPerDay * (period.To.DayNumber - period.From.DayNumber);

        var halls = rows.Select(row => ToHallReport(row, availableHours)).ToList();
        var total = new HallsReportTotal(
            halls.Sum(h => h.Bookings),
            halls.Sum(h => h.BookedHours),
            halls.Sum(h => h.RentRevenue),
            halls.Sum(h => h.ServicesRevenue),
            halls.Sum(h => h.TotalRevenue));

        return new HallsReport(period.From, period.To, workingHoursPerDay, halls, total);
    }

    public async Task<ServicesReport> GetServicesReportAsync(ReportPeriod period, CancellationToken cancellationToken)
    {
        await _validator.EnsureValidAsync(period, cancellationToken);

        var services = await _reports.GetServiceRowsAsync(
            period.From.ToDateTime(TimeOnly.MinValue),
            period.To.ToDateTime(TimeOnly.MinValue),
            cancellationToken);

        return new ServicesReport(period.From, period.To, services, services.Sum(s => s.Revenue));
    }

    private static HallReport ToHallReport(HallReportRow row, decimal availableHours)
    {
        var bookedHours = Math.Round(row.BookedMinutes / 60m, 2);
        var occupancy = availableHours == 0 ? 0 : Math.Round(bookedHours / availableHours * 100, 2);

        return new HallReport(
            row.HallId,
            row.Name,
            row.IsActive,
            row.Bookings,
            bookedHours,
            occupancy,
            row.RentRevenue,
            row.ServicesRevenue,
            row.RentRevenue + row.ServicesRevenue);
    }
}
