using FluentValidation;

namespace HallRental.Application.Reports.ServicesReport;

public class GetServicesReportQueryValidator : AbstractValidator<GetServicesReportQuery>
{
    public GetServicesReportQueryValidator()
    {
        Include(new ReportPeriodValidator());
    }
}
