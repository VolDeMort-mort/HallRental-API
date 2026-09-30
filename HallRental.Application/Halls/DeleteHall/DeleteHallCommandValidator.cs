using FluentValidation;

namespace HallRental.Application.Halls.DeleteHall;

public class DeleteHallCommandValidator : AbstractValidator<DeleteHallCommand>
{
    public DeleteHallCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
