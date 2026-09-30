namespace HallRental.Application.Bookings;

/// <summary>A service included in a booking, with the price it had at the booking moment.</summary>
/// <param name="ServiceId">Id of the hall service.</param>
/// <param name="Name">Service name.</param>
/// <param name="Price">Price in UAH at the booking moment; later price changes don't affect it.</param>
public record BookedServiceDto(Guid ServiceId, string Name, decimal Price);

/// <summary>Booking confirmation with the cost breakdown.</summary>
/// <param name="Id">Booking id.</param>
/// <param name="HallId">Id of the booked hall.</param>
/// <param name="HallName">Name of the booked hall.</param>
/// <param name="ClientId">The person who made the booking.</param>
/// <param name="Start">Start of the rent, hall's local time.</param>
/// <param name="End">End of the rent, hall's local time.</param>
/// <param name="Services">Picked services with their prices.</param>
/// <param name="RentPrice">Hall rent in UAH with the time-of-day discounts and markups applied.</param>
/// <param name="ServicesPrice">Sum of the picked services in UAH.</param>
/// <param name="TotalPrice">RentPrice + ServicesPrice.</param>
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
