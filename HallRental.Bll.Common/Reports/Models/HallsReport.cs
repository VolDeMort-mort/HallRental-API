namespace HallRental.Bll.Common.Reports.Models;

public record HallsReport(
    DateOnly From,
    DateOnly To,
    decimal WorkingHoursPerDay,
    IReadOnlyList<HallReport> Halls,
    HallsReportTotal Total);
