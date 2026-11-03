namespace HallRental.Common.DTOs.Halls;

public record CreateHallDto(
    string Name,
    int Capacity,
    decimal PricePerHour,
    IReadOnlyList<CreateServiceDto> Services);
