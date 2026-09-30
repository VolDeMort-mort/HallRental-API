namespace HallRental.Application.Reports.HallsReport;

/// <summary>
/// Raw sums of one hall, aggregated by the database. The handler turns them into <see cref="HallReportDto"/>,
/// adding what depends on the tariff zones (occupancy).
/// </summary>
public record HallReportRow(
    Guid HallId,
    string Name,
    bool IsActive,
    int Bookings,
    int BookedMinutes,
    decimal RentRevenue,
    decimal ServicesRevenue);
