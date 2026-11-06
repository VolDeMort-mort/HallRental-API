using FluentValidation;
using HallRental.Bll.Common.Halls.Models;
using HallRental.Bll.Common.Shared.Constants;

namespace HallRental.Bll.Halls.Validators;

internal class HallValidator : AbstractValidator<Hall>
{
    public HallValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(EntityLimits.HallNameMaxLength);
        RuleFor(x => x.Capacity).GreaterThan(0);
        RuleFor(x => x.PricePerHour).GreaterThan(0);
        RuleFor(x => x.Services).NotNull();
        RuleForEach(x => x.Services).SetValidator(new ServiceValidator());
    }
}
