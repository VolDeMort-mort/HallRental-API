using FluentValidation;
using HallRental.Bll.Common.Halls.Models;
using HallRental.Bll.Common.Shared.Constants;

namespace HallRental.Bll.Halls.Validators;

internal class ServiceValidator : AbstractValidator<Service>
{
    public ServiceValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(EntityLimits.ServiceNameMaxLength);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}
