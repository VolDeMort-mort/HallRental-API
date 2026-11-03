namespace HallRental.Api.DTOs.Halls;

public record ServiceDto(Guid Id, string Name, decimal Price);

public record HallDto(Guid Id, string Name, int Capacity, decimal PricePerHour, IReadOnlyList<ServiceDto> Services);
