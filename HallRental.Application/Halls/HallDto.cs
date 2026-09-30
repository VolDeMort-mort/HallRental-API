namespace HallRental.Application.Halls;

public record ServiceDto(Guid Id, string Name, decimal Price);

/// <summary>Carries the ids the client needs to book the hall and pick its services.</summary>
public record HallDto(Guid Id, string Name, int Capacity, decimal PricePerHour, IReadOnlyList<ServiceDto> Services);
