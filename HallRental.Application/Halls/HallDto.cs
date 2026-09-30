namespace HallRental.Application.Halls;

/// <summary>The price is charged once per booking.</summary>
public record ServiceDto(Guid Id, string Name, decimal Price);

/// <summary>PricePerHour is the base rent before time-of-day discounts and markups.</summary>
public record HallDto(Guid Id, string Name, int Capacity, decimal PricePerHour, IReadOnlyList<ServiceDto> Services);
