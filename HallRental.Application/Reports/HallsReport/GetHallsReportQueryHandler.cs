using MediatR;
using HallRental.Application.Interfaces;

namespace HallRental.Application.Reports.HallsReport;

public class GetHallsReportQueryHandler : IRequestHandler<GetHallsReportQuery, HallsReportDto>
{
    private readonly IReportQueries _reports;
    private readonly IHoursPricingRepository _pricing;

    public GetHallsReportQueryHandler(IReportQueries reports, IHoursPricingRepository pricing)
    {
        _reports = reports;
        _pricing = pricing;
    }

    public async Task<HallsReportDto> Handle(GetHallsReportQuery request, CancellationToken cancellationToken)
    {
        var from = request.From.ToDateTime(TimeOnly.MinValue);
        var to = request.To.ToDateTime(TimeOnly.MinValue);

        // The heavy part (counting and summing bookings) is done by the database
        var rows = await _reports.GetHallRowsAsync(from, to, cancellationToken);

        // Working hours come from the tariff zones, so the report follows any change of the zones
        var zones = await _pricing.GetAllAsync(cancellationToken);
        var workingHoursPerDay = zones.Sum(z => ToHours(z.To - z.From));
        var availableHours = workingHoursPerDay * (request.To.DayNumber - request.From.DayNumber);

        var halls = rows.Select(row => ToDto(row, availableHours)).ToList();
        var total = new HallsReportTotalDto(
            halls.Sum(h => h.Bookings),
            halls.Sum(h => h.BookedHours),
            halls.Sum(h => h.RentRevenue),
            halls.Sum(h => h.ServicesRevenue),
            halls.Sum(h => h.TotalRevenue));

        return new HallsReportDto(request.From, request.To, workingHoursPerDay, halls, total);
    }

    private static HallReportDto ToDto(HallReportRow row, decimal availableHours)
    {
        var bookedHours = Math.Round(row.BookedMinutes / 60m, 2);
        var occupancy = availableHours == 0 ? 0 : Math.Round(bookedHours / availableHours * 100, 2);

        return new HallReportDto(
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

    private static decimal ToHours(TimeSpan time) => (decimal)time.Ticks / TimeSpan.TicksPerHour;
}
