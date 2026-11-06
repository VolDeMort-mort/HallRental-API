namespace HallRental.Bll.Common.Reports.Models;

public record HallsReportTotal(
    int Bookings,
    decimal BookedHours,
    decimal RentRevenue,
    decimal ServicesRevenue,
    decimal TotalRevenue);
    