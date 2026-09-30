namespace HallRental.Application.Bookings;

/// <summary>A booked service with the price it had at the booking moment.</summary>
public record BookedServiceDto(Guid ServiceId, string Name, decimal Price);

/// <summary>Booking confirmation; the rent already includes the time-of-day discounts and markups.</summary>
public record BookingDto(
    Guid Id,
    Guid HallId,
    string HallName,
    string ClientId,
    DateTime Start,
    DateTime End,
    IReadOnlyList<BookedServiceDto> Services,
    decimal RentPrice,
    decimal ServicesPrice,
    decimal TotalPrice);
