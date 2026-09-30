using FluentValidation;

namespace HallRental.Application.Halls.GetHallById;

public class GetHallByIdQueryValidator : AbstractValidator<GetHallByIdQuery>
{
    public GetHallByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
