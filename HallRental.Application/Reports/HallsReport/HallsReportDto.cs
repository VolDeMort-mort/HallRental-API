namespace HallRental.Application.Reports.HallsReport;

/// <summary>How one hall performed during the period.</summary>
/// <param name="HallId">Hall id.</param>
/// <param name="Name">Hall name.</param>
/// <param name="IsActive">False for a deleted hall: its past revenue is still real, so it stays in the report.</param>
/// <param name="Bookings">Number of bookings that start in the period.</param>
/// <param name="BookedHours">Hours booked in total.</param>
/// <param name="OccupancyPercent">Booked hours out of the working hours of the whole period, in percent.</param>
/// <param name="RentRevenue">Rent in UAH with the time-of-day discounts and markups applied.</param>
/// <param name="ServicesRevenue">Services in UAH at the prices of the booking moment.</param>
/// <param name="TotalRevenue">RentRevenue + ServicesRevenue.</param>
public record HallReportDto(
    Guid HallId,
    string Name,
    bool IsActive,
    int Bookings,
    decimal BookedHours,
    decimal OccupancyPercent,
    decimal RentRevenue,
    decimal ServicesRevenue,
    decimal TotalRevenue);

/// <summary>Sums over all halls.</summary>
/// <param name="Bookings">Number of bookings.</param>
/// <param name="BookedHours">Hours booked in total.</param>
/// <param name="RentRevenue">Rent in UAH.</param>
/// <param name="ServicesRevenue">Services in UAH.</param>
/// <param name="TotalRevenue">RentRevenue + ServicesRevenue.</param>
public record HallsReportTotalDto(
    int Bookings,
    decimal BookedHours,
    decimal RentRevenue,
    decimal ServicesRevenue,
    decimal TotalRevenue);

/// <summary>
/// Bookings, occupancy and revenue of every hall: shows which halls are idle and which earn the most.
/// Active halls are listed even without bookings; deleted halls only if they had bookings in the period.
/// </summary>
/// <param name="From">First day of the period.</param>
/// <param name="To">Day after the period.</param>
/// <param name="WorkingHoursPerDay">Taken from the tariff zones (06:00–23:00 gives 17).</param>
/// <param name="Halls">One row per hall, ordered by name.</param>
/// <param name="Total">Sums over all halls.</param>
public record HallsReportDto(
    DateOnly From,
    DateOnly To,
    decimal WorkingHoursPerDay,
    IReadOnlyList<HallReportDto> Halls,
    HallsReportTotalDto Total);
