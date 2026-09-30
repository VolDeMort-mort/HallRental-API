using FluentValidation;

namespace HallRental.Application.Reports;

/// <summary>Rules shared by every report; each report validator includes them.</summary>
public class ReportPeriodValidator : AbstractValidator<IReportPeriod>
{
    // A cap on the period, so one request can't make the database aggregate the whole history
    public const int MaxDays = 366;

    public ReportPeriodValidator()
    {
        RuleFor(x => x.To).GreaterThan(x => x.From).WithMessage("To has to be later than From");
        RuleFor(x => x.To)
            .Must((period, to) => to.DayNumber - period.From.DayNumber <= MaxDays)
            .WithMessage($"The period can't be longer than {MaxDays} days");
    }
}
