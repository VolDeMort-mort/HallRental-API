namespace HallRental.Bll.Common.Reports.Models;

public record HallReport(
    Guid HallId,
    string Name,
    bool IsActive,
    int Bookings,
    decimal BookedHours,
    decimal OccupancyPercent,
    decimal RentRevenue,
    decimal ServicesRevenue,
    decimal TotalRevenue);
