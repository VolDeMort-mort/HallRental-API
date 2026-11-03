namespace HallRental.Services.Web.DTOs.Bookings;

public record CreateBookingDto(
    Guid HallId,
    DateTime Start,
    TimeSpan Duration,
    IReadOnlyList<Guid> ServiceIds);
