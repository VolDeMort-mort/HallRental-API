namespace HallRental.Common.DTOs.Reports;

/// <summary>[From, To) in days, like a rental period; a booking belongs to the period by its start.</summary>
public record ReportPeriodDto(DateOnly From, DateOnly To);
