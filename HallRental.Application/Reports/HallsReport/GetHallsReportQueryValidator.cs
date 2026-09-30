using FluentValidation;

namespace HallRental.Application.Reports.HallsReport;

public class GetHallsReportQueryValidator : AbstractValidator<GetHallsReportQuery>
{
    public GetHallsReportQueryValidator()
    {
        Include(new ReportPeriodValidator());
    }
}
