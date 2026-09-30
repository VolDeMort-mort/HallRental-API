namespace HallRental.Application.Bookings;

public record BookedServiceDto(Guid ServiceId, string Name, decimal Price);

/// <summary>Booking confirmation with the cost breakdown.</summary>
public record BookingDto(
    Guid Id,
    string HallName,
    DateTime Start,
    DateTime End,
    IReadOnlyList<BookedServiceDto> Services,
    decimal RentPrice,
    decimal ServicesPrice,
    decimal TotalPrice);
