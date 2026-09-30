namespace HallRental.Application.Reports;

/// <summary>
/// A report period in days: <see cref="From"/> inclusive, <see cref="To"/> exclusive,
/// the same half-open rule as a rental period. A booking belongs to the period by its start.
/// </summary>
public interface IReportPeriod
{
    DateOnly From { get; }
    DateOnly To { get; }
}
