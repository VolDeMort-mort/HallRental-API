using FluentValidation;
using HallRental.Domain.Entities;

namespace HallRental.Application.Halls.AddService;

public class AddServiceCommandValidator : AbstractValidator<AddServiceCommand>
{
    public AddServiceCommandValidator()
    {
        RuleFor(x => x.HallId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(Service.MaxNameLength);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}
