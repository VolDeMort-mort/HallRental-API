using FluentValidation;
using HallRental.Domain.Entities;

namespace HallRental.Application.Halls.UpdateHall;

public class UpdateHallCommandValidator : AbstractValidator<UpdateHallCommand>
{
    public UpdateHallCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(Hall.MaxNameLength);
        RuleFor(x => x.Capacity).GreaterThan(0);
        RuleFor(x => x.PricePerHour).GreaterThan(0);
    }
}
