using FluentValidation;

namespace HallRental.Application.Bookings.CreateBooking;

public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        RuleFor(x => x.HallId).NotEmpty();

        // Only a safety cap against DateTime overflow; working hours are checked by the domain
        RuleFor(x => x.Duration)
            .GreaterThan(TimeSpan.Zero)
            .LessThanOrEqualTo(TimeSpan.FromDays(1)).WithMessage("Duration can't be longer than 1 day");

        RuleFor(x => x.ServiceIds).NotNull();
        RuleForEach(x => x.ServiceIds).NotEmpty();
    }
}
