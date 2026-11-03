namespace HallRental.Api.DTOs.Halls;

public record UpdateHallDto(string Name, int Capacity, decimal PricePerHour);
