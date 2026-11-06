namespace HallRental.Bll.Common.Reports.Models;

public record HallReportRow(
    Guid HallId,
    string Name,
    bool IsActive,
    int Bookings,
    int BookedMinutes,
    decimal RentRevenue,
    decimal ServicesRevenue);
