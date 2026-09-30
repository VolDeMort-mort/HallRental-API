using FluentValidation;

namespace HallRental.Application.Halls.GetAvailableHalls;

public class GetAvailableHallsQueryValidator : AbstractValidator<GetAvailableHallsQuery>
{
    public GetAvailableHallsQueryValidator()
    {
        RuleFor(x => x.End).GreaterThan(x => x.Start).WithMessage("End has to be later than Start");
        RuleFor(x => x.MinCapacity).GreaterThan(0);
    }
}
