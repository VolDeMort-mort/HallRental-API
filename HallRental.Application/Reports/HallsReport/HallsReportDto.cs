namespace HallRental.Application.Reports.HallsReport;

/// <summary>One hall for the period; occupancy is booked hours out of all working hours of the period.</summary>
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

public record HallsReportTotalDto(
    int Bookings,
    decimal BookedHours,
    decimal RentRevenue,
    decimal ServicesRevenue,
    decimal TotalRevenue);

/// <summary>Active halls are listed even without bookings, deleted ones only if they had bookings in the period.</summary>
public record HallsReportDto(
    DateOnly From,
    DateOnly To,
    decimal WorkingHoursPerDay,
    IReadOnlyList<HallReportDto> Halls,
    HallsReportTotalDto Total);
