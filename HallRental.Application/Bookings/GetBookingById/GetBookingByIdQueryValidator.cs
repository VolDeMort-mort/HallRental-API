using FluentValidation;

namespace HallRental.Application.Bookings.GetBookingById;

public class GetBookingByIdQueryValidator : AbstractValidator<GetBookingByIdQuery>
{
    public GetBookingByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
