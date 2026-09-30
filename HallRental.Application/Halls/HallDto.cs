namespace HallRental.Application.Halls;

/// <summary>A service offered by a hall.</summary>
/// <param name="Id">Pass it in serviceIds when booking the hall.</param>
/// <param name="Name">Service name.</param>
/// <param name="Price">Price in UAH, charged once per booking.</param>
public record ServiceDto(Guid Id, string Name, decimal Price);

/// <summary>A conference hall with the ids the client needs to book it and pick its services.</summary>
/// <param name="Id">Pass it as hallId when booking.</param>
/// <param name="Name">Hall name.</param>
/// <param name="Capacity">Maximum number of people.</param>
/// <param name="PricePerHour">Base rent per hour in UAH, before time-of-day discounts and markups.</param>
/// <param name="Services">Services the hall offers.</param>
public record HallDto(Guid Id, string Name, int Capacity, decimal PricePerHour, IReadOnlyList<ServiceDto> Services);
