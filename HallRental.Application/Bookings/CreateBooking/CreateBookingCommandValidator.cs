using FluentValidation;

namespace HallRental.Application.Bookings.CreateBooking;

public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        RuleFor(x => x.HallId).NotEmpty();

        // The upper limit is only a safety cap (DateTime overflow, pointless load on pricing).
        // Working hours are checked by the domain: they come from the tariff zones, not from here.
        RuleFor(x => x.Duration)
            .GreaterThan(TimeSpan.Zero)
            .LessThanOrEqualTo(TimeSpan.FromDays(1)).WithMessage("Duration can't be longer than 1 day");

        RuleFor(x => x.ServiceIds).NotNull();
        RuleForEach(x => x.ServiceIds).NotEmpty();
    }
}
