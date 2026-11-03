using FluentValidation;

namespace HallRental.Bll.Halls.Validators;

internal class AvailableHallsFilterValidator : AbstractValidator<AvailableHallsFilter>
{
    public AvailableHallsFilterValidator()
    {
        RuleFor(x => x.End).GreaterThan(x => x.Start).WithMessage("End has to be later than Start");
        RuleFor(x => x.MinCapacity).GreaterThan(0);
    }
}
