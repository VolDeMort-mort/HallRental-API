namespace HallRental.Services.Web.DTOs.Reports;

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

public record HallsReportDto(
    DateOnly From,
    DateOnly To,
    decimal WorkingHoursPerDay,
    IReadOnlyList<HallReportDto> Halls,
    HallsReportTotalDto Total);
