using FluentValidation;
using HallRental.Domain.Entities;

namespace HallRental.Application.Halls.AddHall;

public class AddHallCommandValidator : AbstractValidator<AddHallCommand>
{
    public AddHallCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(Hall.MaxNameLength);
        RuleFor(x => x.Capacity).GreaterThan(0);
        RuleFor(x => x.PricePerHour).GreaterThan(0);
        RuleFor(x => x.Services).NotNull();
        RuleForEach(x => x.Services).ChildRules(service =>
        {
            service.RuleFor(s => s.Name).NotEmpty().MaximumLength(Service.MaxNameLength);
            service.RuleFor(s => s.Price).GreaterThan(0);
        });
    }
}
