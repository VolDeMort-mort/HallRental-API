namespace HallRental.Application.Reports.HallsReport;

/// <summary>Sums of one hall as the database returns them, before occupancy is calculated.</summary>
public record HallReportRow(
    Guid HallId,
    string Name,
    bool IsActive,
    int Bookings,
    int BookedMinutes,
    decimal RentRevenue,
    decimal ServicesRevenue);
