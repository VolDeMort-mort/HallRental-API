namespace HallRental.Api.DTOs.Bookings;

public record BookedServiceDto(Guid ServiceId, string Name, decimal Price);

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
