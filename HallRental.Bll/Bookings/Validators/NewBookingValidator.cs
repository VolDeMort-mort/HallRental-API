using FluentValidation;
using HallRental.Bll.Common.Bookings.Models;

namespace HallRental.Bll.Bookings.Validators;

internal class NewBookingValidator : AbstractValidator<NewBooking>
{
    public NewBookingValidator()
    {
        RuleFor(x => x.HallId).NotEmpty();

        // Only a safety cap against DateTime overflow; working hours are checked by PricingPolicy
        RuleFor(x => x.Duration)
            .GreaterThan(TimeSpan.Zero)
            .LessThanOrEqualTo(TimeSpan.FromDays(1)).WithMessage("Duration can't be longer than 1 day");

        RuleFor(x => x.ServiceIds).NotNull();
        RuleForEach(x => x.ServiceIds).NotEmpty();
    }
}
