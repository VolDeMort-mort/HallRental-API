namespace HallRental.Application.Reports;

// [From, To) in days, like a rental period; a booking belongs to the period by its start
public interface IReportPeriod
{
    DateOnly From { get; }
    DateOnly To { get; }
}
